/*
  https://stackoverflow.com/a/2117523
*/
// eslint-disable-next-line max-classes-per-file
export const generateUuidv4 = () => ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(
  /[018]/g,
  // eslint-disable-next-line no-bitwise,no-mixed-operators
  (c) => (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16),
);

export const generateURL = (path) => `${window.location.origin}${window.location.pathname}${path}`;

export const isEmptyValue = (val) => (val === '' || val === undefined || val === null || val === [] || val === {});

const MINS_MS = 60 * 1000;
const HOURS_MS = MINS_MS * 60;
const DAYS_MS = HOURS_MS * 24;

export const timeAgo = ({ minutes = 0, hours = 0, days = 0 }) => new Date(
  Date.now() - minutes * MINS_MS - hours * HOURS_MS - days * DAYS_MS,
);

export class TimeRange {
  constructor(startTime, endTime) {
    this.startTime = startTime;
    this.endTime = endTime;
  }

  getStartTime() {
    return this.startTime;
  }

  getEndTime() {
    return this.endTime;
  }

  getText() {
    return `${this.startTime.toISOString().substring(0, 16)}Z - ${this.endTime
      .toISOString()
      .substring(0, 16)}Z`;
  }
}

export class TimeAgoRange extends TimeRange {
  constructor(startTimeAgo, endTimeAgo = { days: 0 }) {
    super(null, null);
    this.startTimeAgo = startTimeAgo;
    this.endTimeAgo = endTimeAgo;
  }

  getStartTime() {
    return timeAgo(this.startTimeAgo);
  }

  getEndTime() {
    return timeAgo(this.endTimeAgo);
  }

  getText() {
    const shortForm = (value) => {
      if (value?.minutes > 0) {
        return `${value.minutes} minutes`;
      }

      if (value?.days > 0) {
        return `${value.days} days`;
      }

      if (value?.hours > 0) {
        return `${value.hours} hours`;
      }
      return 'now';
    };
    const startText = shortForm(this.startTimeAgo);
    const endText = shortForm(this.endTimeAgo);

    if (endText === 'now') {
      return `Last ${startText}`;
    }
    return `${startText} ago - ${endText} ago`;
  }
}
