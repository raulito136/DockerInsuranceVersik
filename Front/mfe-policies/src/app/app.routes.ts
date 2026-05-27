import { Route } from '@angular/router';
import { remoteRoutes } from './remote-entry/entry.routes';
import { PolicyListComponent } from './features/policies/policy-list/policy-list.component';
import { PolicyFormComponent } from './features/policies/policy-form/policy-form.component';

export const appRoutes: Route[] = [
  { path: '', component: PolicyListComponent },
  { path: 'new', component: PolicyFormComponent },
];
