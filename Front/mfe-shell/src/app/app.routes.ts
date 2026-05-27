import { Route } from '@angular/router';
import { loadRemote } from '@module-federation/enhanced/runtime';
import { WelcomeComponent } from './pages/welcome/welcome.component';

export const appRoutes: Route[] = [
  {
    path: '',
    component: WelcomeComponent,
    pathMatch: 'full', // URL must be EMPTY
  },
  {
    path: 'claims',
    loadChildren: () =>
      loadRemote<any>('mfe-claims/Routes')
        .then((m) => {
          console.log('Claims module loaded:', m);
          return m.remoteRoutes;
        })
        .catch((err) => {
          console.error('Fatal error loading mfe-claims:', err);
          throw err;
        }),
  },
  {
    path: 'policies',
    loadChildren: () =>
      loadRemote<any>('mfe-policies/Routes')
        .then((m) => {
          console.log('Policies module loaded:', m);
          return m.policyRoutes;
        })
        .catch((err) => {
          console.error('Fatal error loading mfe-policies:', err);
          throw err;
        }),
  },
  {
    path: 'policy-holders',
    loadChildren: () =>
      loadRemote<any>('mfe-policies/Routes')
        .then((m) => {
          console.log('Policies Holder module loaded:', m);
          return m.policyHolderRoutes;
        })
        .catch((err) => {
          console.error('Fatal error loading mfe-policies:', err);
          throw err;
        }),
  },
  {
    path: 'reference-data',
    loadChildren: () =>
      loadRemote<any>('mfe-reference-data/Routes')
        .then((m) => {
          console.log('Reference Data module loaded:', m);
          return m.remoteRoutes;
        })
        .catch((err) => {
          console.error('Fatal error loading mfe-reference-data:', err);
          throw err;
        }),
  },
];
