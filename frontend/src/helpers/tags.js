// TODO: maintain local tags
export const retrieveRecentTags = async () => [];

export const tagsFromData = (data) => data.TagEvent?.Tags || [];

export const tagsDiff = (setA, setB) => {
  const diff = new Set(setA);
  // eslint-disable-next-line no-restricted-syntax
  for (const el of setB) {
    diff.delete(el);
  }
  return [...diff];
};

export const tagsIntersect = (setA, setB) => {
  const intersect = new Set();
  const currentSet = new Set(setA);
  // eslint-disable-next-line no-restricted-syntax
  for (const el of setB) {
    if (currentSet.has(el)) {
      intersect.add(el);
    }
  }
  return [...intersect];
};
