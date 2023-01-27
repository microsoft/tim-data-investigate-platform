<template>
  <v-dialog
    v-model="dialog"
    max-width="800px"
    persistent
    hide-overlay
    :retain-focus="false"
    no-click-animation
  >
    <v-card>
      <v-form
        ref="form"
        v-model="valid"
        lazy-validation
      >
        <v-card-title class="popup-header">
          <span class="headline">Customise Tag Events</span>
        </v-card-title>
        <v-card-subtitle>
          {{ events.length }} event(s) selected
        </v-card-subtitle>
        <v-card-text>
          <v-container fluid no-gutters>
            <v-row>
              <v-col class="py-1">
                <v-select
                  v-model="determination"
                  :items="['Malicious', 'Suspicious', 'Benign']"
                  :disabled="determinationIgnore"
                  :rules="[(v) => determinationIgnore || !!v || 'Determination cannot be empty.']"
                  label="Determination"
                />
              </v-col>
              <v-col class="py-1" cols="2">
                <v-select
                  v-model="determinationAction"
                  :items="[actions.Ignore, actions.Override, actions.Remove]"
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col class="py-1">
                <v-text-field
                  v-model="comment"
                  label="Comment"
                  :disabled="commentIgnore"
                  :rules="[(v) => commentIgnore || !!v || 'Comment cannot be empty.' ]"
                />
              </v-col>
              <v-col class="py-1" cols="2">
                <v-select
                  v-model="commentAction"
                  :items="[actions.Ignore, actions.Override, actions.Append]"
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col class="py-1">
                <v-autocomplete
                  v-model="selectedTags"
                  :items="tags"
                  chips
                  deletable-chips
                  dense
                  hide-details
                  hide-selected
                  label="Tags"
                  multiple
                  :search-input.sync="search"
                  :disabled="tagAction === actions.Ignore"
                >
                  <template
                    v-if="tagAction !== actions.Remove"
                    #prepend-item
                  >
                    <v-list-item
                      ripple
                      :disabled="!search"
                      @mousedown.prevent
                      @click="onClickTagSelect"
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          {{ search || "Type to add new tag..." }}
                        </v-list-item-title>
                        <v-list-item-subtitle>Custom tag</v-list-item-subtitle>
                      </v-list-item-content>
                    </v-list-item>
                    <v-divider class="mt-2" />
                  </template>
                  <template #item="{ item }">
                    <v-list-item-content>
                      <v-list-item-title>
                        {{ item }}
                      </v-list-item-title>
                      <v-list-item-subtitle v-if="item in existingTagsObj">
                        Exists in {{ existingTagsObj[item].length }} event(s)
                      </v-list-item-subtitle>
                      <v-list-item-subtitle
                        v-else-if="recentTags.includes(item)"
                      >
                        Recent tag
                      </v-list-item-subtitle>
                    </v-list-item-content>
                  </template>
                </v-autocomplete>
              </v-col>
              <v-col class="py-1" cols="2">
                <v-select
                  v-model="tagAction"
                  :items="[
                    actions.Ignore,
                    actions.Append,
                    actions.Remove,
                    actions.Override,
                  ]"
                  @change="onClickChangeTagAction"
                />
              </v-col>
            </v-row>
          </v-container>
          <ul
            v-for="(error, index) in errors"
            :key="`error-${index}`"
            class="error--text"
          >
            <li>{{ error }}</li>
          </ul>
          <ul
            v-for="(modification, index) in modifications"
            :key="`mod-${index}`"
          >
            <li>{{ modification }}</li>
          </ul>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="blue darken-1"
            text
            @click="dialog = false"
          >
            Close
          </v-btn>
          <v-btn
            color="blue darken-1"
            text
            @click="onSaveTagEvent()"
          >
            Save
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>
<script>
import eventBus from '@/helpers/eventBus';
import { createComments, createTags, saveEvents } from '@/helpers/apiClient';
import {
  presetTags,
  retrieveRecentTags,
  tagsDiff,
  tagsFromData,
  tagsIntersect,
} from '@/helpers/tags';

const Actions = {
  Ignore: 'Ignore',
  Override: 'Override',
  Append: 'Append',
  Remove: 'Remove',
};

export default {
  name: 'TagEventDialog',
  data: () => ({
    dialog: false,
    determination: null,
    comment: null,
    onSuccess: null,
    search: null,
    recentTags: [],
    selectedTags: [],
    pinnedTags: [],
    commentAction: Actions.Override,
    determinationAction: Actions.Override,
    tagAction: Actions.Ignore,
    events: [],
    valid: true,
    errors: [],
    modifications: [],
    modifiedEvents: [],
  }),
  computed: {
    tags() {
      if (this.tagAction === Actions.Remove) {
        return [...new Set([...this.selectedTags, ...this.existingTags])];
      }
      return [
        ...new Set([
          ...this.selectedTags,
          ...this.existingTags,
          ...this.recentTags,
          ...presetTags(),
        ]),
      ];
    },
    existingTags() {
      return this.events.map((data) => tagsFromData(data)).flat();
    },
    existingTagsObj() {
      return this.events.reduce((obj, data) => {
        tagsFromData(data).forEach((tag) => {
          if (!obj[tag]) {
            // eslint-disable-next-line no-param-reassign
            obj[tag] = [];
          }
          obj[tag].push(data);
        });
        return obj;
      }, {});
    },
    actions() {
      return Actions;
    },
    unsavedEvents() {
      return this.events.filter((data) => data.TagEvent?.IsSaved !== true);
    },
    determinationIgnore() {
      return this.determinationAction === Actions.Ignore
        || this.determinationAction === Actions.Remove;
    },
    commentIgnore() {
      return this.commentAction === Actions.Ignore;
    },
  },
  watch: {
    selectedTags() {
      this.updateModifications();
    },
  },
  async beforeCreate() {
    eventBus.$on('create:tag-event-dialog', async ({ events, onSuccess }) => {
      this.dialog = true;
      this.events = events;
      this.onSuccess = onSuccess;
      this.selectedTags = [];
      this.errors = [];
      this.modifications = [];
      this.tagAction = Actions.Ignore;
      this.recentTags = await retrieveRecentTags();
    });
  },
  methods: {
    onClickTagSelect() {
      this.selectedTags = [...new Set([this.search, ...this.selectedTags])];
      this.search = null;
    },
    onClickChangeTagAction() {
      this.updateModifications();
    },
    updateModifications() {
      this.modifications = [];
      if (this.selectedTags.length === 0) {
        return;
      }

      if (this.tagAction === Actions.Append) {
        this.selectedTags.forEach((tag) => {
          const modCount = this.events.filter(
            (data) => !tagsFromData(data).includes(tag),
          );
          if (modCount.length > 0) {
            this.modifications.push(
              `Adding ${tag} to ${modCount.length} event(s).`,
            );
          }
        });
      } else if (this.tagAction === Actions.Remove) {
        this.selectedTags.forEach((tag) => {
          const modCount = this.events.filter((data) => tagsFromData(data).includes(tag));
          if (modCount.length > 0) {
            this.modifications.push(
              `Removing ${tag} from ${modCount.length} event(s).`,
            );
          }
        });
      } else if (this.tagAction === Actions.Override) {
        this.selectedTags.forEach((tag) => {
          const modCount = this.events.filter(
            (data) => !tagsFromData(data).includes(tag),
          );
          if (modCount.length > 0) {
            this.modifications.push(
              `Adding ${tag} to ${modCount.length} event(s).`,
            );
          }
        });
        // Remove tags not in tag selection
        const removeTagCounts = this.events.reduce((obj, data) => {
          tagsFromData(data)
            .filter((tag) => !this.selectedTags.includes(tag))
            .forEach((tag) => {
              if (!(tag in obj)) {
                // eslint-disable-next-line no-param-reassign
                obj[tag] = 0;
              }
              // eslint-disable-next-line no-param-reassign
              obj[tag] += 1;
            });
          return obj;
        }, {});
        Object.entries(removeTagCounts).forEach(([tag, count]) => {
          if (count > 0) {
            this.modifications.push(`Removing ${tag} from ${count} event(s).`);
          }
        });
      }
    },
    validate() {
      this.errors = [];

      if (this.commentAction === Actions.Ignore
        && this.determinationAction === Actions.Ignore
        && this.tagAction === Actions.Ignore) {
        this.errors.push('At least one action should be selected.');
      }

      if (this.determinationAction === Actions.Ignore
        && this.unsavedEvents.length > 0) {
        this.errors.push(
          `Determination is missing from ${this.unsavedEvents.length} event(s) and cannot be ignored.`,
        );
      }

      if (this.tagAction !== Actions.Ignore && this.selectedTags.length === 0) {
        this.errors.push('Tags cannot be empty.');
      }

      if (this.determinationAction === Actions.Remove
        && (this.commentAction !== Actions.Ignore || this.tagAction !== Actions.Ignore)) {
        this.errors.push(
          'Comment and tag actions must be ignored when removing determination.',
        );
      }

      return this.errors.length === 0;
    },
    getComment(oldComment) {
      switch (this.commentAction) {
        case Actions.Ignore:
          return oldComment;
        case Actions.Override:
          return this.comment;
        case Actions.Append:
          return `${oldComment} ${this.comment}`.trim();
        default:
          return '';
      }
    },
    getDetermination(oldDetermination) {
      switch (this.determinationAction) {
        case Actions.Ignore:
          return oldDetermination;
        case Actions.Override:
          return this.determination;
        default:
          return '';
      }
    },
    async removeDetermination() {
      console.log('Removing tag events...');

      try {
        eventBus.$emit('show:snackbar', {
          message: 'Removing tag events...',
        });

        const comments = [];
        const newEvents = this.events.map((data) => {
          comments.push({
            eventId: data.EventId,
            comment: 'removed',
            determination: 'removed',
            isDeleted: true,
          });
          return {
            ...data,
            TagEvent: null,
          };
        });
        await createComments(comments);
        this.events = newEvents;
        this.onSuccess(this.events);

        eventBus.$emit('show:snackbar', {
          message: 'Tag events successfully removed.',
          color: 'success',
          icon: 'mdi-check',
        });
      } catch (err) {
        eventBus.$emit('show:snackbar', {
          message: `Removing tag events failed: ${err.toString()}`,
          color: 'error',
          icon: 'mdi-alert',
        });
        console.log(err);
      }

      this.dialog = false;
    },
    async saveEvents() {
      const events = [];
      const newEvents = this.events.map((data) => {
        if (data.TagEvent?.IsSaved !== true) {
          events.push({
            eventId: data.EventId,
            eventTime: data.EventTime,
            data,
          });
        }

        return {
          ...data,
          TagEvent: {
            ...data.TagEvent,
            IsSaved: true,
          },
        };
      });

      if (events.length > 0) {
        await saveEvents(events);
        this.events = newEvents;
      }
    },
    async saveComments() {
      const comments = [];
      const newEvents = this.events.map((data) => {
        const comment = this.getComment(data.TagEvent?.Comment || '');
        const determination = this.getDetermination(data.TagEvent?.Determination).toLowerCase();

        comments.push({
          eventId: data.EventId,
          comment,
          determination,
          isDeleted: false,
        });

        return {
          ...data,
          TagEvent: {
            ...data.TagEvent,
            Comment: comment,
            Determination: determination,
          },
        };
      });
      await createComments(comments);
      this.events = newEvents;
    },
    async addTags() {
      const addTagList = [];
      const newEvents = this.events.map((data) => {
        const addTags = tagsDiff(this.selectedTags, tagsFromData(data));

        addTagList.push(
          ...addTags.map((tag) => ({
            eventId: data.EventId,
            tag,
            isDeleted: false,
          })),
        );

        return {
          ...data,
          TagEvent: {
            ...data.TagEvent,
            Tags: [...tagsFromData(data), ...addTags],
          },
        };
      });
      await createTags(addTagList);
      this.events = newEvents;
    },
    async removeTags() {
      const removeTagList = [];

      const newEvents = this.events.map((data) => {
        const removeTags = tagsIntersect(tagsFromData(data), this.selectedTags);

        removeTagList.push(
          ...removeTags.map((tag) => ({
            eventId: data.EventId,
            tag,
            isDeleted: true,
          })),
        );

        return {
          ...data,
          TagEvent: {
            ...data.TagEvent,
            Tags: tagsDiff(tagsFromData(data), this.selectedTags),
          },
        };
      });
      await createTags(removeTagList);
      this.events = newEvents;
    },
    async setTags() {
      const modifyTags = [];
      const newEvents = this.events.map((data) => {
        const addTags = tagsDiff(this.selectedTags, tagsFromData(data));
        const removeTags = tagsDiff(tagsFromData(data), this.selectedTags);

        modifyTags.push(
          ...addTags.map((tag) => ({
            eventId: data.EventId,
            tag,
            isDeleted: false,
          })),
        );
        modifyTags.push(
          ...removeTags.map((tag) => ({
            eventId: data.EventId,
            tag,
            isDeleted: true,
          })),
        );

        return {
          ...data,
          TagEvent: {
            ...data.TagEvent,
            Tags: this.selectedTags,
          },
        };
      });
      await createTags(modifyTags);
      this.events = newEvents;
    },
    async onSaveTagEvent() {
      if (!this.$refs.form.validate() || !this.validate()) {
        return false;
      }

      if (this.determinationAction === Actions.Remove) {
        return this.removeDetermination();
      }

      console.log('Saving events...');

      eventBus.$emit('show:snackbar', {
        message: 'Customising tag events...',
      });

      try {
        if (this.determinationAction === Actions.Override) {
          await this.saveEvents();
        }

        if (this.determinationAction !== Actions.Ignore || this.commentAction !== Actions.Ignore) {
          await this.saveComments();
        }

        if (this.tagAction === Actions.Append) {
          await this.addTags();
        } else if (this.tagAction === Actions.Remove) {
          await this.removeTags();
        } else if (this.tagAction === Actions.Override) {
          await this.setTags();
        }

        this.onSuccess(this.events);

        this.determination = null;
        this.comment = null;
        this.selectedTags = [];

        eventBus.$emit('show:snackbar', {
          message: 'Tag events successfully customised.',
          color: 'success',
          icon: 'mdi-check',
        });
      } catch (err) {
        eventBus.$emit('show:snackbar', {
          message: `Customisation of tag events failed: ${err.toString()}`,
          color: 'error',
          icon: 'mdi-alert',
        });
        console.log(err);
      }

      this.dialog = false;
      return true;
    },
  },
};
</script>
