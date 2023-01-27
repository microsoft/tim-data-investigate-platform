import localforage from 'localforage';

const dataStore = localforage.createInstance({
  driver: localforage.INDEXEDDB,
  storeName: 'row_results',
});

export const saveDataStore = async (uuid, rowData) => {
  await dataStore.setItem(uuid, rowData);
};

export const loadDataStore = async (uuid) => dataStore.getItem(uuid);

export const deleteDataStore = async (uuid) => dataStore.removeItem(uuid);
