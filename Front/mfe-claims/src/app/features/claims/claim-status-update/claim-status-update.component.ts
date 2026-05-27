import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ClaimService } from '../../../services/claim.service';
import { ReferenceDataService } from '../../../services/reference-data.service';
import { FormComponent, SelectComponent, ButtonComponent, LoaderComponent, ConfirmationComponent } from '@policy-system/ui';

@Component({
  selector: 'app-claim-status-update',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, FormComponent, SelectComponent, ButtonComponent, LoaderComponent, ConfirmationComponent],
  templateUrl: './claim-status-update.component.html'
})
export class ClaimStatusUpdateComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private claimService = inject(ClaimService);
  private refDataService = inject(ReferenceDataService);
  
  claimId!: number;
  claimNumber = signal<string>('');
  currentStatus = signal<string>('');
  newStatus = 'UNDER_REVIEW';
  statusOptions: { value: string, content: string }[] = [];
  loading = signal<boolean>(true);
  showModal = signal(false);
  modalTitle = signal('');
  modalText = signal('');

  private readonly validTransitions: Record<string, string[]> = {
    'SUBMITTED': ['UNDER_REVIEW'],
    'UNDER_REVIEW': ['APPROVED', 'REJECTED'],
    'APPROVED': ['PAID'],
    'REJECTED': [],
    'PAID': []
  };

  ngOnInit() {
    this.claimId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    
    this.claimService.getClaimById(this.claimId).subscribe(claimRes => {
      this.claimNumber.set(claimRes.data.claimNumber);
      this.currentStatus.set(claimRes.data.statusCode);
      
      this.refDataService.getClaimStatuses().subscribe(allStatuses => {
        const allowedCodes = this.validTransitions[claimRes.data.statusCode] || [];
        this.statusOptions = allStatuses
          .filter(s => allowedCodes.includes(s.code))
          .map(s => ({ value: s.code, content: s.name }));
        
        if (this.statusOptions.length > 0) {
          this.newStatus = this.statusOptions[0].value as string;
        }
        
        this.loading.set(false);
      });
    });
  }

  actualizarEstado() {
    this.claimService.updateStatus(this.claimId, this.newStatus, 'AdminUser').subscribe({
      next: () => this.router.navigate(['../'], { relativeTo: this.route }),
      error: (err) => {
        const errors = err.error?.errors || [{ message: 'Invalid transition' }];
        this.showAlert('Error updating status', errors.map((e: any) => e.message).join(', '));
      }
    });
  }

  showAlert(title: string, text: string) {
    this.modalTitle.set(title);
    this.modalText.set(text);
    this.showModal.set(true);
  }

  onModalClose() {
    this.showModal.set(false);
  }

  cancelar() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}