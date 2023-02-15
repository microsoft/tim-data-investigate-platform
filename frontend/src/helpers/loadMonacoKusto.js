// Original source code: https://github.com/grafana/grafana/pull/33528

let promise = null;

const loadMonacoKusto = () => new Promise((resolve) => {
  window.monacoKustoResolvePromise = resolve;

  const script = document.createElement('script');
  script.innerHTML = 'require([\'vs/language/kusto/monaco.contribution\'], function() { window.monacoKustoResolvePromise(); });';

  document.body.appendChild(script);
});

const loadScript = (script) => new Promise((resolve, reject) => {
  let scriptEl;

  if (typeof script === 'string') {
    scriptEl = document.createElement('script');
    scriptEl.src = script;
  } else {
    scriptEl = script;
  }

  scriptEl.onload = () => resolve();
  scriptEl.onerror = (err) => reject(err);
  document.body.appendChild(scriptEl);
});

const loadScripts = async () => {
  const scripts = [
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/kusto.javascript.client.min.js`,
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/newtonsoft.json.min.js`,
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/Kusto.Language.Bridge.min.js`,
  ];

  await loadScript(`${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/bridge.min.js`);

  const scriptPromises = scripts.map((script) => loadScript(script));
  await Promise.all(scriptPromises);

  await loadMonacoKusto();
};

const loadKusto = async () => {
  if (promise === null) {
    promise = loadScripts();
  }
  return promise;
};

export default loadKusto;
