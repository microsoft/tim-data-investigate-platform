import Handlebars from 'handlebars';

const lookupTableFunctions = `
// --- LookupTableFunctions ---
let getTagEvents=(T:(EventId:string)) { 
  let EventIds=materialize(T | distinct EventId);
  let Events=EventIds
  | join kind=leftouter (
    cluster('${import.meta.env.VITE_KUSTO_CLUSTER}').database('${
  import.meta.env.VITE_KUSTO_DATABASE
}').SavedEvent
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, *) by EventId
    | project EventId, IsSaved=true
  ) on EventId
  | join kind=leftouter (
    cluster('${import.meta.env.VITE_KUSTO_CLUSTER}').database('${
  import.meta.env.VITE_KUSTO_DATABASE
}').EventTag
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, IsDeleted) by EventId, Tag
    | where not(IsDeleted)
    | summarize Tags=make_set(Tag) by EventId
    | project EventId, Tags
  ) on EventId
  | join kind=leftouter (
    cluster('${import.meta.env.VITE_KUSTO_CLUSTER}').database('${
  import.meta.env.VITE_KUSTO_DATABASE
}').EventComment
    | where EventId in (EventIds)
    | sort by DateTimeUtc desc
    | summarize arg_max(DateTimeUtc, Determination, IsDeleted, Comment), 
      Comments=make_list(pack("CreatedBy", CreatedBy, "Comment", Comment, "Determination", Determination, "DateTimeUtc", DateTimeUtc)) 
      by EventId
    | where not(IsDeleted)
    | project EventId, Determination, Comment, Comments
  ) on EventId
  | project-away EventId1, EventId2, EventId3
  | extend TagEvent=pack_all()
  | project EventId, TagEvent;
  T
  | join kind=inner Events on EventId
  | project-away EventId1
};
`;
Handlebars.registerPartial('lookupTableFunctions', lookupTableFunctions);
Handlebars.registerHelper('array', (items) => items?.map((item) => `@'${item}'`).join(','));

// eslint-disable-next-line import/prefer-default-export
export class QueryTemplate {
  constructor(config) {
    this.uuid = config.uuid;
    this.menu = config.menu;
    this.summary = config.summary;
    this.queryType = config.queryType;
    this.path = config.path;
    this.cluster = config.cluster;
    this.database = config.database;
    this.params = config.params || {};
    this.fields = config.fields || {};
    this.query = config.query;
    this.columns = config.columns || [];
    this.columnId = config.columnId || null;
  }

  getDefaultParams() {
    return Object.keys(this.params).reduce((obj, k) => {
      // eslint-disable-next-line no-param-reassign
      obj[k] = this.params[k].default || '';
      return obj;
    }, {});
  }

  buildSummary(params) {
    const template = Handlebars.compile(this.summary, { noEscape: true });
    return template(params);
  }

  validateData(data, multipleData) {
    if (this.fields === undefined) {
      return true;
    }

    return Object.keys(this.fields).every((fieldName) => {
      if (this.fields[fieldName].type === 'multiple') {
        const fieldFrom = this.fields[fieldName].from;
        return (
          multipleData.length > 0
          && multipleData.every(
            (e) => fieldFrom in e && e[fieldFrom] !== null && e[fieldFrom] !== '',
          )
        );
      } if (this.fields[fieldName].type === 'match') {
        const regex = new RegExp(this.fields[fieldName].regex);
        return Object.keys(data).some(
          (col) => regex.test(col) && data[col] !== null && data[col] !== '',
        );
      }
      return fieldName in data;
    });
  }

  isDataComplete(data) {
    if (this.fields === undefined) {
      return true;
    }

    return Object.keys(this.fields).every((fieldName) => {
      if (this.fields[fieldName].type === 'multiple') {
        return data[fieldName]?.length > 0;
      } if (this.fields[fieldName].type === 'match') {
        return data[fieldName]?.length === 1;
      }
      return (
        fieldName in data && data[fieldName] !== null && data[fieldName] !== ''
      );
    });
  }

  buildParams(data, multipleData) {
    const newParams = Object.keys(this.params).reduce((obj, paramName) => {
      // eslint-disable-next-line no-param-reassign
      obj[paramName] = this.params[paramName].default;
      return obj;
    }, {});
    Object.keys(this.fields).forEach((fieldName) => {
      if (this.fields[fieldName].type === 'multiple') {
        newParams[fieldName] = multipleData
          .map((e) => e[this.fields[fieldName].from] ?? '')
          .filter((e) => e !== null && e !== '');
      } else if (this.fields[fieldName].type === 'match') {
        const regex = new RegExp(this.fields[fieldName].regex);
        newParams[fieldName] = Object.keys(data)
          .filter(
            (col) => regex.test(col) && data[col] !== null && data[col] !== '',
          )
          .map((col) => ({ column: col, value: data[col] }));
      } else {
        newParams[fieldName] = data[fieldName] ?? '';
      }
    });
    return newParams;
  }

  buildCluster(params) {
    const template = Handlebars.compile(this.cluster, { noEscape: true });
    return template(params);
  }

  buildQuery(params) {
    const template = Handlebars.compile(this.query, { noEscape: true });
    return template(params);
  }
}
