import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyHolderService } from '../../../services/policy-holder.service';
import { PolicyHolder } from '../../../models/policy-holder.model';
import {
  CardComponent,
  ButtonComponent,
  LoaderComponent,
  TableComponent,
  InlineErrorComponent,
  ConfirmationComponent,
} from '@policy-system/ui';

@Component({
  selector: 'app-policy-holder-detail',
  standalone: true,
  imports: [
    CommonModule,
    CardComponent,
    ButtonComponent,
    LoaderComponent,
    TableComponent,
    InlineErrorComponent,
    ConfirmationComponent,
  ],
  templateUrl: './policy-holder-detail.component.html',
  styleUrl: './policy-holder-detail.component.css',
})
export class PolicyHolderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private service = inject(PolicyHolderService);

  policyHolder?: PolicyHolder;
  isLoading = false;
  errorMessage: string = '';

  // Confirmation panel
  title: string = '';
  text: string = '';
  isConfirming = false;
  private resolveConfirmation: ((value: boolean) => void) | null = null; // Store the promise of the confirmation

  policyColumns = [
    { key: 'policyNumber', label: 'Policy Number' },
    { key: 'policyTypeCode', label: 'Type' },
    { key: 'status', label: 'Status' },
    { key: 'startDate', label: 'Start Date' },
  ];

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.loadPolicyHolder(+idParam);
    }
  }

  loadPolicyHolder(id: number): void {
    this.isLoading = true;
    this.service.getPolicyHolderById(id).subscribe({
      next: (response) => {
        this.policyHolder = response.data;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.errors?.map((e: any) => e.message) || [
          'Failed to load details.',
        ];
        this.isLoading = false;
      },
    });
  }

  onBack(): void {
    this.router.navigate(['/policy-holders']);
  }

  onEdit(): void {
    if (this.policyHolder) {
      this.router.navigate(['/policy-holders', this.policyHolder.id, 'edit']);
    }
  }

  onPolicySelect(row: any): void {
    this.router.navigate(['/policies', row.id]);
  }

  async onDelete(id: number): Promise<void> {
    const confirmationValue = await this.askForConfirmation(
      'Confirm Delete',
      'Are you sure you want to delete this policy? This action can not be undone.'
    );

    if (confirmationValue) {
      this.isLoading = true;

      this.service.deletePolicyHolder(id).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/policy-holders']);
        },
        error: (err) => {
          this.errorMessage =
            err.error?.errors?.map((e: any) => e.message).join(', ') ||
            'Error during policy remove.';
          this.isLoading = false;
        },
      });
    }
  }

  // Methods needed for confirmation
  handleConfirmation(input: boolean): void {
    this.isConfirming = false;

    // if a promise waiting, resolve with the confirmation component Output
    if (this.resolveConfirmation) {
      this.resolveConfirmation(input);
      this.resolveConfirmation = null; // Clean for any other attempt
    }
  }

  askForConfirmation(title: string, text: string): Promise<boolean> {
    this.title = title;
    this.text = text;
    this.isConfirming = true;

    return new Promise<boolean>((resolve) => {
      this.resolveConfirmation = resolve;
    });
  }
}
