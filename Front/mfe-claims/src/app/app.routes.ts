import { Routes } from '@angular/router';
import { ClaimListComponent } from './features/claims/claim-list/claim-list.component';
import { ClaimFormComponent } from './features/claims/claim-form/claim-form.component';
import { ClaimDetailComponent } from './features/claims/claim-detail/claim-detail.component';
import { ClaimStatusUpdateComponent } from './features/claims/claim-status-update/claim-status-update.component';

export const routes: Routes = [
  { path: '', component: ClaimListComponent },
  { path: 'new', component: ClaimFormComponent },
  { path: ':id', component: ClaimDetailComponent },
  { path: ':id/status', component: ClaimStatusUpdateComponent }
];