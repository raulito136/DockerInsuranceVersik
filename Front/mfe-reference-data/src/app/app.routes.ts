import { Route } from '@angular/router';
import { ClaimStatusListComponent } from './features/claim-statuses/claim-status-list/claim-status-list.component';
import { ClaimStatusFormComponent } from './features/claim-statuses/claim-status-form/claim-status-form.component';
import { PolicyTypeListComponent } from './features/policy-types/policy-type-list/policy-type-list.component';
import { CoverageTypeListComponent } from './features/coverage-types/coverage-type-list/coverage-type-list.component';
import { PolicyTypeFormComponent } from './features/policy-types/policy-type-form/policy-type-form.component';
import { CoverageTypeFormComponent } from './features/coverage-types/coverage-type-form/coverage-type-form.component';
import { RegionListComponent } from './features/regions/region-list/region-list.component';
import { RegionFormComponent } from './features/regions/region-form/region-form.component';

export const appRoutes: Route[] = [
  {
    path: 'claim-statuses',
    component: ClaimStatusListComponent
  },
  {
    path: 'claim-statuses/new',
    component: ClaimStatusFormComponent
  },
  {
    path: 'claim-statuses/:id/edit',
    component: ClaimStatusFormComponent
  },
  {
    path:'policy-types', component: PolicyTypeListComponent
  },
  {
    path:'policy-types/new', component: PolicyTypeFormComponent
  },
  {
    path:'policy-types/:id/edit', component: PolicyTypeFormComponent
  },
  {
    path:'coverage-types', component: CoverageTypeListComponent
  },
  {
    path:'coverage-types/new', component: CoverageTypeFormComponent
  },
  {
    path:'coverage-types/:id/edit', component: CoverageTypeFormComponent
  },
  {
    path:'regions', component: RegionListComponent
  },
  {
    path:'regions/new', component: RegionFormComponent
  },
  {
    path:'regions/:id/edit', component: RegionFormComponent
  },
  {path: '', redirectTo: 'claim-statuses', pathMatch: 'full' }
];
