import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyService } from '../../../services/policy.service';
import { Policy } from '../../../models/policy.model';
import {
  CardComponent,
  ButtonComponent,
  LoaderComponent,
  InlineErrorComponent,
  ConfirmationComponent,
} from '@policy-system/ui';

@Component({
  selector: 'app-policy-detail',
  standalone: true,
  imports: [
    CommonModule,
    CardComponent,
    ButtonComponent,
    LoaderComponent,
    CurrencyPipe,
    DatePipe,
    InlineErrorComponent,
    ConfirmationComponent,
  ],
  templateUrl: './policy-detail.component.html',
  styleUrl: './policy-detail.component.css',
})
export class PolicyDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private service = inject(PolicyService);

  policy?: Policy;
  isLoading = false;
  errorMessage: string = '';

  // Confirmation panel
  title: string = '';
  text: string = '';
  isConfirming = false;
  private resolveConfirmation: ((value: boolean) => void) | null = null; // Store the promise of the confirmation

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.loadPolicy(+idParam);
    }
  }

  loadPolicy(id: number): void {
    this.isLoading = true;
    this.service.getPolicyById(id).subscribe({
      next: (response) => {
        this.policy = response.data;
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
    this.router.navigate(['/policies']);
  }

  onEdit(): void {
    if (this.policy) {
      this.router.navigate(['/policies', this.policy.id, 'edit']);
    }
  }

  onViewHolder(): void {
    if (this.policy) {
      this.router.navigate(['/policy-holders', this.policy.policyHolderId]);
    }
  }

  async onDelete(id: number): Promise<void> {
    const confirmationValue = await this.askForConfirmation(
      'Confirm Delete',
      'Are you sure you want to delete this policy? This action can not be undone.'
    );

    if (confirmationValue) {
      this.isLoading = true;

      this.service.deletePolicy(id).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/policies']);
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
