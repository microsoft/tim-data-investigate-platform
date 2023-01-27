<template>
  <v-dialog
    :value="dialog"
    persistent
    max-width="800px"
  >
    <v-card>
      <v-card-title>
        <span class="headline">Query Help</span>
      </v-card-title>
      <v-card-text>
        <div class="text-subtitle-2">
          Required Fields
        </div>
        <v-simple-table>
          <template #default>
            <thead>
              <tr>
                <th>Field</th>
                <th>Type</th>
                <th>Comment</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td><code>EventId</code></td>
                <td>Tagging</td>
                <td>Unique identifier for this event.</td>
              </tr>
              <tr>
                <td><code>EventTime</code></td>
                <td>Tagging,Queries</td>
                <td>
                  Set to an interesting time e.g. EventTime, FileCreationTime.
                </td>
              </tr>
              <tr>
                <td><code>Cluster</code></td>
                <td>Queries</td>
                <td>
                  Set to the cluster where the data exists. Required for almost
                  all pivots.
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
        <div class="text-subtitle-2 mt-5">
          Time Range Parameters
        </div>
        <div class="text--primary my-2">
          The <code>StartTime</code> and <code>EndTime</code> time range
          parameters are injected into the Kusto query using
          <code>query_parameters</code>.
        </div>
        <v-card class="my-2 pa-2" outlined>
          <pre class="code">
declare query_parameters(<strong>StartTime</strong>:datetime, <strong>EndTime</strong>:datetime);
...
| where Timestamp between (<strong>StartTime</strong> .. <strong>EndTime</strong>)</pre>
        </v-card>
        <div class="text-subtitle-2 mt-5">
          Tagged Events
        </div>
        <div class="text--primary my-2">
          This helper function can be used to highlight events that have been
          tagged by yourself or other analysts. The table must have a column
          named <code>EventId</code>.
        </div>
        <v-card class="my-2 pa-2" outlined>
          <pre class="code">
let getTagEvents=(T:(EventId:string)) {
  let EventIds=materialize(T | distinct EventId);
  let Events=EventIds
  | join kind=leftouter (
    cluster('{{ defaultCluster }}').database('{{ defaultDatabase }}').SavedEvent
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, *) by EventId
    | project EventId, IsSaved=true
  ) on EventId
  | join kind=leftouter (
    cluster('{{ defaultCluster }}').database('{{ defaultDatabase }}').EventTag
    | where EventId in (EventIds)
    | summarize arg_max(DateTimeUtc, IsDeleted) by EventId, Tag
    | where not(IsDeleted)
    | summarize Tags=make_set(Tag) by EventId
    | project EventId, Tags
  ) on EventId
  | join kind=leftouter (
    cluster('{{ defaultCluster }}').database('{{
              defaultDatabase
            }}').EventComment
    | where EventId in (EventIds)
    | sort by DateTimeUtc desc
    | summarize arg_max(DateTimeUtc, Determination, IsDeleted, Comment),
      Comments=make_list(pack(
            "CreatedBy", CreatedBy,
            "Comment", Comment,
            "Determination", Determination,
            "DateTimeUtc", DateTimeUtc))
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
CreateFileEvents
| take 1
| invoke GetTagEvents()</pre>
        </v-card>
        <div class="text-subtitle-2 mt-5">
          Examples
        </div>
        <v-card class="my-2 pa-2" outlined>
          <pre class="code">{{ defaultNewQuery }}}</pre>
        </v-card>
        <v-card class="my-2 pa-2" outlined>
          <pre class="code">More examples...</pre>
        </v-card>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="blue darken-1"
          text
          @click="$emit('update:dialog', false)"
        >
          Close
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import { defaultNewQuery } from '@/helpers/displayComponent';

export default {
  name: 'QueryHelperDialog',
  props: {
    dialog: Boolean,
  },
  computed: {
    defaultNewQuery() {
      return defaultNewQuery();
    },
    defaultCluster() {
      return import.meta.env.VITE_KUSTO_CLUSTER;
    },
    defaultDatabase() {
      return import.meta.env.VITE_KUSTO_DATABASE;
    },
  },
};
</script>
