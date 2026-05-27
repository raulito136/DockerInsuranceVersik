import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { ClaimService } from '../../../services/claim.service';
import { Claim } from '../../../models/claim.model';
import { CommentListComponent } from '../../comments/comment-list/comment-list.component';
import { AuditTrailComponent } from '../../audit/audit-trail/audit-trail.component';
import { CardComponent, TabsComponent, ButtonComponent, LoaderComponent, ConfirmationComponent } from '@policy-system/ui';

@Component({
  selector: 'app-claim-detail',
  imports: [CommonModule, RouterModule, CommentListComponent, AuditTrailComponent, CardComponent, TabsComponent, ButtonComponent, LoaderComponent, ConfirmationComponent],
  templateUrl: './claim-detail.component.html'
})
export class ClaimDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private claimService = inject(ClaimService);
  private router = inject(Router);
  currentTab: string = 'Comments';
  claim = signal<Claim | null>(null);
  loading = signal<boolean>(true);
  
  showDeleteModal = signal(false);
  isDeleting = signal(false);

  ngOnInit() {
    this.refreshData();
  }

  refreshData() {
    this.loading.set(true);
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.claimService.getClaimById(id).subscribe({
      next: (res) => {
        this.claim.set(res.data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  confirmDelete() {
    this.showDeleteModal.set(true);
  }

  onDeleteConfirm(confirmed: boolean) {
    this.showDeleteModal.set(false);
    if (confirmed && this.claim()) {
      this.isDeleting.set(true);
      this.claimService.deleteClaim(this.claim()!.id).subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.router.navigate(['..'], { relativeTo: this.route });
        },
        error: (err: any) => {
          this.isDeleting.set(false);
          alert('Error deleting claim: ' + (err.error?.message || 'Unknown error'));
        }
      });
    }
  }
}