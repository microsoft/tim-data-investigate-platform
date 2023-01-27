<template>
  <v-menu
    v-model="dateMenu"
    :close-on-content-click="false"
    offset-y
    min-width="400"
    @input="showCustom = CustomTimePicker.None"
  >
    <template #activator="{ on, attrs }">
      <v-btn
        v-bind="attrs"
        rounded
        small
        text
        v-on="on"
      >
        Time range:
        {{ timeRange !== null ? timeRange.getText() : "Invalid time" }}
      </v-btn>
    </template>
    <v-card v-if="showCustom === CustomTimePicker.DateRange">
      <v-card-title class="popup-header">
        <span class="headline">Custom date range</span>
      </v-card-title>
      <v-card-text>
        <v-menu
          v-model="startDatePickerMenu"
          :close-on-content-click="false"
          transition="scale-transition"
          offset-y
          bottom
          min-width="auto"
        >
          <template #activator="{ on, attrs }">
            <v-text-field
              v-model="fieldStartDate"
              label="Start date"
              prepend-icon="mdi-calendar"
              v-bind="attrs"
              v-on="on"
            />
          </template>
          <v-date-picker
            v-model="fieldStartDate"
            :max="new Date().toISOString()"
            @input="startDatePickerMenu = false"
          />
        </v-menu>
        <v-text-field
          v-model="fieldStartTime"
          label="Start time"
          prepend-icon="mdi-clock"
          :rules="[(v) => (!!v && v.search(datetimeRegex) !== -1) || 'Please input time (HH:MM)']"
          required
        />
        <v-menu
          v-model="endDatePickerMenu"
          :close-on-content-click="false"
          transition="scale-transition"
          offset-y
          bottom
          min-width="auto"
        >
          <template #activator="{ on, attrs }">
            <v-text-field
              v-model="fieldEndDate"
              label="End date"
              prepend-icon="mdi-calendar"
              v-bind="attrs"
              v-on="on"
            />
          </template>
          <v-date-picker
            v-model="fieldEndDate"
            :max="new Date().toISOString()"
            @input="endDatePickerMenu = false"
          />
        </v-menu>
        <v-text-field
          v-model="fieldEndTime"
          label="End time"
          prepend-icon="mdi-clock"
          :rules="[(v) => (!!v && v.search(datetimeRegex) !== -1) || 'Please input time (HH:MM)']"
          required
        />
        <v-row v-if="errors.length > 0">
          <ul
            v-for="(error, index) in errors"
            :key="`error-${index}`"
            class="error--text"
          >
            <li>{{ error }}</li>
          </ul>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="darken-1"
          text
          @click="dateMenu = false"
        >
          Cancel
        </v-btn>
        <v-btn
          color="blue darken-1"
          text
          @click="onApplyDateRange()"
        >
          Apply
        </v-btn>
      </v-card-actions>
    </v-card>
    <v-card v-else-if="showCustom === CustomTimePicker.TimePeriod">
      <v-form>
        <v-card-title class="popup-header">
          <span class="headline">Custom time period</span>
        </v-card-title>
        <v-card-text>
          <v-row>
            <v-col>
              <v-text-field
                v-model="startTimePeriodInput"
                label="Start time period"
                prepend-icon="mdi-clock"
              />
            </v-col>
            <v-col>
              <v-select
                v-model="startTimePeriodUnits"
                :items="['minutes', 'hours', 'days']"
                label="Time units"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                v-model="endTimePeriodInput"
                label="End time period"
                prepend-icon="mdi-clock"
                :disabled="endTimePeriodUnits === 'now'"
              />
            </v-col>
            <v-col>
              <v-select
                v-model="endTimePeriodUnits"
                :items="['now', 'minutes', 'hours', 'days']"
                label="Time units"
              />
            </v-col>
          </v-row>
          <v-row v-if="errors.length > 0">
            <ul
              v-for="(error, index) in errors"
              :key="`error-${index}`"
              class="error--text"
            >
              <li>{{ error }}</li>
            </ul>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="darken-1"
            text
            @click="dateMenu = false"
          >
            Cancel
          </v-btn>
          <v-btn
            color="blue darken-1"
            text
            @click="onApplyTimePeriod()"
          >
            Apply
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
    <v-list v-else>
      <v-list-item-group
        v-model="quickSelect"
        active-class=""
        mandatory
      >
        <v-list-item @click="showCustom = CustomTimePicker.DateRange">
          <v-list-item-content>
            <v-list-item-title>Custom Date Range</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="showCustom = CustomTimePicker.TimePeriod">
          <v-list-item-content>
            <v-list-item-title>Custom Time Period</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item
          v-for="(item, i) in timeMenuItems"
          :key="i"
          @click="onClickTimeMenu(i)"
        >
          <v-list-item-content>
            <v-list-item-title v-text="item.getText()" />
          </v-list-item-content>
        </v-list-item>
      </v-list-item-group>
    </v-list>
  </v-menu>
</template>
<script>
import { TimeAgoRange, TimeRange } from '@/helpers/utils';

const timeMenuItems = () => [
  new TimeAgoRange({ minutes: 15 }),
  new TimeAgoRange({ minutes: 30 }),
  new TimeAgoRange({ hours: 1 }),
  new TimeAgoRange({ hours: 24 }),
  new TimeAgoRange({ days: 7 }),
  new TimeAgoRange({ days: 30 }),
  new TimeAgoRange({ days: 90 }),
];

const getDateString = (date) => `${date.getUTCFullYear()}-${String(date.getUTCMonth() + 1).padStart(2, '0')}-${String(date.getUTCDate()).padStart(2, '0')}`;
const getTimeString = (date) => `${String(date.getUTCHours()).padStart(2, '0')}:${String(date.getUTCMinutes()).padStart(2, '0')}`;

const CustomTimePicker = {
  DateRange: 'DateRange',
  TimePeriod: 'TimePeriod',
  None: 'None',
};

export default {
  name: 'TimeSelection',
  props: {
    timeRange: {
      type: Object,
      default: null,
    },
  },
  data: () => ({
    showCustom: CustomTimePicker.None,
    quickSelect: null,
    editTime: null,
    fieldStartDate: getDateString(new Date()),
    fieldStartTime: '00:00',
    fieldEndDate: getDateString(new Date()),
    fieldEndTime: '00:00',
    startDatePickerMenu: null,
    endDatePickerMenu: null,
    dateMenu: null,
    startTimePeriodInput: null,
    startTimePeriodUnits: 'minutes',
    endTimePeriodInput: null,
    endTimePeriodUnits: 'now',
    errors: [],
  }),
  computed: {
    timeMenuItems() {
      return timeMenuItems();
    },
    CustomTimePicker() {
      return CustomTimePicker;
    },
    datetimeRegex() {
      return /^([0-1]?[0-9]|2[0-3]):[0-5][0-9]Z?$/;
    },
  },
  mounted() {
    if (this.timeRange !== null) {
      this.fieldStartDate = getDateString(this.timeRange.getStartTime());
      this.fieldStartTime = getTimeString(this.timeRange.getStartTime());

      this.fieldEndDate = getDateString(this.timeRange.getEndTime());
      this.fieldEndTime = getTimeString(this.timeRange.getEndTime());
    } else {
      // eslint-disable-next-line prefer-destructuring
      this.editTime = this.timeMenuItems[0];
      this.updateDate();
    }
  },
  methods: {
    onApplyDateRange() {
      try {
        this.editTime = new TimeRange(
          new Date(`${this.fieldStartDate}T${this.fieldStartTime}Z`),
          new Date(`${this.fieldEndDate}T${this.fieldEndTime}Z`),
        );

        if (this.validate()) {
          this.updateDate();
          this.dateMenu = false;
        }
        return true;
      } catch (e) {
        return false;
      }
    },
    onApplyTimePeriod() {
      try {
        if (this.endTimePeriodUnits === 'now') {
          this.editTime = new TimeAgoRange({
            [this.startTimePeriodUnits]: this.startTimePeriodInput,
          });
        } else {
          this.editTime = new TimeAgoRange(
            { [this.startTimePeriodUnits]: this.startTimePeriodInput },
            { [this.endTimePeriodUnits]: this.endTimePeriodInput },
          );
        }

        if (this.validate()) {
          this.updateDate();
          this.dateMenu = false;
        }
        return true;
      } catch (e) {
        return false;
      }
    },
    onClickTimeMenu(index) {
      this.editTime = this.timeMenuItems[index];
      this.updateDate();

      this.dateMenu = false;
    },
    validate() {
      this.errors = [];
      if (this.editTime.getStartTime() >= this.editTime.getEndTime()) {
        this.errors.push('Start time must be prior to end time');
      }
      return this.errors.length === 0;
    },
    updateDate() {
      this.$emit('update:time-range', this.editTime);
    },
  },
};
</script>

<style></style>
