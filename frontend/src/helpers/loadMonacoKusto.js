// Original source code: https://github.com/grafana/grafana/pull/33528

const scripts = [
  [`${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/bridge.min.js`],
  [
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/kusto.javascript.client.min.js`,
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/newtonsoft.json.min.js`,
    `${import.meta.env.BASE_URL}monaco-editor/min/vs/language/kusto/Kusto.Language.Bridge.min.js`,
  ],
];

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

const loadKusto = async () => {
  const promise = Promise.resolve();

  for (const parallelScripts of scripts) {
    await promise;

    // Load all these scripts in parallel, then wait for them all to finish before continuing
    // to the next iteration
    const allPromises = parallelScripts
      .filter((src) => !document.querySelector(`script[src="${src}"]`))
      .map((src) => loadScript(src));

    await Promise.all(allPromises);
  }

  await loadMonacoKusto();
};

export default loadKusto;
