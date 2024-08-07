import Handlebars from 'handlebars';
import runtimeConfig from '@/helpers/runtimeConfig';

const getTagEvents = `
let getTagEvents=(T:(EventId:string)) { 
  let EventIds=materialize(T | distinct EventId);
  let Events=EventIds
  | lookup (
    cluster('${runtimeConfig.tagCluster}').database('${runtimeConfig.tagDatabase}').SavedEvent
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, *) by EventId
    | project EventId
  ) on EventId
  | lookup (
    cluster('${runtimeConfig.tagCluster}').database('${runtimeConfig.tagDatabase}').EventTag
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, IsDeleted) by EventId, Tag
    | where not(IsDeleted)
    | summarize Tags=make_set(Tag) by EventId
    | project EventId, Tags
  ) on EventId
  | lookup (
    cluster('${runtimeConfig.tagCluster}').database('${runtimeConfig.tagDatabase}').EventComment
    | where EventId in (EventIds)
    | sort by DateTimeUtc desc
    | summarize arg_max(DateTimeUtc, Determination, IsDeleted, Comment), 
      Comments=make_list(pack("CreatedBy", CreatedBy, "Comment", Comment, "Determination", Determination, "DateTimeUtc", DateTimeUtc)) 
      by EventId
    | where not(IsDeleted)
    | project EventId, Determination, Comment, Comments, IsSaved=true
  ) on EventId
  | extend TagEvent=pack_all()
  | project EventId, TagEvent;
  T
  | join kind=inner Events on EventId
  | project-away EventId1
};
`;
Handlebars.registerPartial('getTagEvents', getTagEvents);
Handlebars.registerHelper('array', (items) => items?.map((item) => `@'${item}'`).join(','));
Handlebars.registerHelper('tagCluster', runtimeConfig.tagCluster);
Handlebars.registerHelper('tagDatabase', runtimeConfig.tagDatabase);

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
      if (this.fields[fieldName]?.type === 'multiple') {
        const fieldFrom = this.fields[fieldName].from;
        return (
          multipleData?.length > 0
          && multipleData.some(
            (e) => fieldFrom in e && e[fieldFrom] !== null && e[fieldFrom] !== '',
          )
        );
      } if (this.fields[fieldName]?.type === 'match') {
        const regex = new RegExp(this.fields[fieldName].regex);
        return Object.keys(data).some(
          (col) => regex.test(col) && data[col] !== null && data[col] !== '',
        );
      }
      return fieldName in data && data[fieldName] !== null && data[fieldName] !== '';
    });
  }

  isDataComplete(data) {
    if (this.fields === undefined) {
      return true;
    }

    return Object.keys(this.fields).every((fieldName) => {
      if (this.fields[fieldName]?.type === 'multiple') {
        return data[fieldName]?.length > 0;
      } if (this.fields[fieldName]?.type === 'match') {
        return typeof data[fieldName] === 'string' || data[fieldName]?.length === 1;
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
      if (this.fields[fieldName]?.type === 'multiple') {
        newParams[fieldName] = multipleData
          .map((e) => e[this.fields[fieldName].from] ?? '')
          .filter((e) => e !== null && e !== '');
      } else if (this.fields[fieldName]?.type === 'match') {
        const regex = new RegExp(this.fields[fieldName].regex);
        const filteredResults = Object.keys(data)
          .filter(
            (col) => regex.test(col) && data[col] !== null && data[col] !== '',);
          if (filteredResults?.length === 1) {
            // If only one element passes the regex test, set `newParams` to that element's string value to automatically populate the field
            newParams[fieldName] = data[filteredResults[0]];
          } else {
              newParams[fieldName] = filteredResults.map((col) => ({ column: col, value: data[col] }));
          }
      
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

  buildDatabase(params) {
    const template = Handlebars.compile(this.database, { noEscape: true });
    return template(params);
  }

  buildQuery(params) {
    const template = Handlebars.compile(this.query, { noEscape: true });
    return template(params);
  }
}
