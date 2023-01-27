import { runKustoQueryPoll } from '@/helpers/queries';

export const presetTags = () => new Set([
  'cat:ransomware',
  'cat:credtheft',
  'cat:ad',
  'cat:privesc',
  'cat:recon',
  'cat:persistence',
  'cat:postexpl',
  'cat:procinject',
  'cat:revshell',
  'cat:latmove',
  'cat:latmove.src',
  'cat:latmove.dst',
  'cat:staging',
  'cat:exfil',
  'cat:rce',
  'cat:c2',
  'cat:backdoor',
  'cat:keylogger',
  'cat:downloader',
  'cat:webshell',
  'cat:payload_delivery',
  'cat:kb.ioc',
  'cat:kb.av',
  'cat:kb.ioa',
  'cat:kb.ta',
  'cat:evasion',
  'cat:evasion.rename',
  'cat:evasion.encoding',
  'cat:evasion.obfuscation',
  'cat:evasion.tampering',
  'mal:cobaltstrike',
  'data:proc',
  'data:file',
  'data:reg',
  'data:net',
  'data:image',
  'data:scan',
  'data:alert',
  'data:etw.amsi',
  'data:etw.av',
  'data:etw.credman',
  'data:etw.cmdlet',
  'data:etw.event19',
  'data:etw.event24',
  'data:etw.event25',
  'data:etw.kl_keystate',
  'data:etw.ldap',
  'data:etw.lnk',
  'data:logon',
  'data:etw.openproc',
  'data:etw.readvmremote',
  'data:etw.task',
  'data:etw.wdav_detection',
  'data:etw.wmi_bind',
]);

const retrieveRecentTagsPromiseTimeOut = 200; // ms
let retrieveRecentTagsPromiseEndTime = null;
let retrieveRecentTagsPromise = null;
export const retrieveRecentTags = async () => {
  if (retrieveRecentTagsPromise === null || Date.now() > retrieveRecentTagsPromiseEndTime) {
    retrieveRecentTagsPromise = runKustoQueryPoll(
      import.meta.env.VITE_KUSTO_CLUSTER,
      import.meta.env.VITE_KUSTO_DATABASE,
      `EventTag
        | where DateTimeUtc >= ago(7d)
        | where not(IsDeleted)
        | summarize tagCount=count() by tostring(Tag)
        | order by tagCount desc
        | take 25`,
    );
    retrieveRecentTagsPromiseEndTime = Date.now() + retrieveRecentTagsPromiseTimeOut;
  }

  const result = await retrieveRecentTagsPromise;

  if (result.data.length > 0) {
    return [...new Set(result.data.map((e) => e.Tag))];
  }
  return [];
};

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
