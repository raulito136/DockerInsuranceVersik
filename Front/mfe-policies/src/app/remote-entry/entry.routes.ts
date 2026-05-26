import { Route } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { RemoteEntryComponent } from './entry.component';
import { PolicyListComponent } from '../features/policies/policy-list/policy-list.component';
import { PolicyFormComponent } from '../features/policies/policy-form/policy-form.component';
import { PolicyDetailComponent } from '../features/policies/policy-detail/policy-detail.component';
import { PolicyHolderListComponent } from '../features/policy-holders/policy-holder-list/policy-holder-list.component';
import { PolicyHolderFormComponent } from '../features/policy-holders/policy-holder-form/policy-holder-form.component';
import { PolicyHolderDetailComponent } from '../features/policy-holders/policy-holder-detail/policy-holder-detail.component';

export const policyRoutes: Route[] = [
  {
    path: '',
    component: RemoteEntryComponent,
    providers: [provideHttpClient()],
    children: [
      { path: '', component: PolicyListComponent },
      { path: 'new', component: PolicyFormComponent },
      { path: ':id', component: PolicyDetailComponent },
      { path: ':id/edit', component: PolicyFormComponent }
    ]
  }
];

export const policyHolderRoutes: Route[] = [
  {
    path: '',
    component: RemoteEntryComponent,
    providers: [provideHttpClient()],
    children: [
      { path: '', component: PolicyHolderListComponent },
      { path: 'new', component: PolicyHolderFormComponent },
      { path: ':id', component: PolicyHolderDetailComponent },
      { path: ':id/edit', component: PolicyHolderFormComponent }
    ]
  }
];

// For standalone running
export const remoteRoutes: Route[] = [
  { path: 'policies', children: policyRoutes },
  { path: 'policy-holders', children: policyHolderRoutes },
  { path: '', redirectTo: 'policies', pathMatch: 'full' }
];
