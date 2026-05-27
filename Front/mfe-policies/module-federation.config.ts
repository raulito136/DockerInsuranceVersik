import { ModuleFederationConfig } from '@nx/webpack';

const config: ModuleFederationConfig = {
  name: 'mfe-policies',
  exposes: {
    './Routes': 'mfe-policies/src/app/remote-entry/entry.routes.ts',
  },
};

export default config;
