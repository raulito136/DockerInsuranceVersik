import { init } from '@module-federation/enhanced/runtime';

fetch('/module-federation.manifest.json')
  .then((res) => res.json())
  .then((definitions) => {
    // transform objects {"mfe-xxxxx": "http://xxxxxxxx"} to the init() wanted format
    const remotes = Object.entries(definitions).map(([name, entry]) => ({
      name,
      entry: `${entry}/mf-manifest.json`,
    }));

    // initialize native engine of Module federation
    init({
      name: 'mfe-shell',
      remotes: remotes,
    });
  })
  .then(() => import('./bootstrap').catch((err) => console.error(err)));
