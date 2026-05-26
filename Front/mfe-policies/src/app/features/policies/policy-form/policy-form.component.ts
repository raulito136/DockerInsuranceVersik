import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyService } from '../../../services/policy.service';
import { PolicyHolderService } from '../../../services/policy-holder.service';
import { ReferenceDataService } from '../../../services/reference-data.service';
import {
  FormComponent,
  InputComponent,
  SelectComponent,
  DatepickerComponent,
  ButtonComponent,
  LoaderComponent,
  InlineErrorComponent,
} from '@policy-system/ui';
import { max } from 'rxjs';

@Component({
  selector: 'app-policy-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormComponent,
    InputComponent,
    SelectComponent,
    DatepickerComponent,
    ButtonComponent,
    LoaderComponent,
    InlineErrorComponent,
  ],
  templateUrl: './policy-form.component.html',
  styleUrl: './policy-form.component.css',
})
export class PolicyFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(PolicyService);
  private phService = inject(PolicyHolderService);
  private refDataService = inject(ReferenceDataService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  form!: FormGroup;
  isEditMode = false;
  policyId?: number;
  isLoading = false;
  isSaving = false;

  policyHolderOptions: { value: string; content: string }[] = [];
  policyTypeOptions: { value: string; content: string }[] = [];
  coverageTypeOptions: { value: string; content: string }[] = [];
  statusOptions = [
    { value: 'ACTIVE', content: 'Active' },
    { value: 'INACTIVE', content: 'Inactive' },
    { value: 'EXPIRED', content: 'Expired' },
    { value: 'CANCELLED', content: 'Cancelled' },
  ];

  globalErrorMessage: string = '';

  ngOnInit(): void {
    this.initForm();
    this.loadDropdownData();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam && idParam !== 'new') {
      this.isEditMode = true;
      this.policyId = +idParam;

      // If edit mode, status is required and policyHolder is disabled/hidden in request
      this.form.addControl(
        'status',
        this.fb.control('ACTIVE', [Validators.required])
      );
      this.form.get('policyHolderId')?.disable();

      this.loadPolicy(this.policyId);
    }
  }

  private initForm(): void {
    const maxDecimal = 9999999999999999;
    this.form = this.fb.group({
      policyHolderId: ['', [Validators.required]],
      policyTypeCode: ['', [Validators.required]],
      coverageTypeCode: ['', [Validators.required]],
      coverageAmount: [
        '',
        [Validators.required, Validators.min(0.01), Validators.max(maxDecimal)],
      ],
      premiumAmount: [
        '',
        [Validators.required, Validators.min(0.01), Validators.max(maxDecimal)],
      ],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
    });
  }

  private loadDropdownData(): void {
    // Load Policy Holders (ideally an autocomplete, but for this project we'll load the first page with a large size)
    this.phService.getPolicyHolders(1, 1000).subscribe((response) => {
      this.policyHolderOptions = response.data.map((ph) => ({
        value: ph.id.toString(),
        content: `${ph.firstName} ${ph.lastName} (ID: ${ph.id})`,
      }));
    });

    this.refDataService.getPolicyTypes().subscribe((types) => {
      this.policyTypeOptions = types.map((t) => ({
        value: t.code,
        content: `${t.name} (${t.code})`,
      }));
    });

    this.refDataService.getCoverageTypes().subscribe((types) => {
      this.coverageTypeOptions = types.map((t) => ({
        value: t.code,
        content: `${t.name} (${t.code})`,
      }));
    });
  }

  private loadPolicy(id: number): void {
    this.isLoading = true;
    this.service.getPolicyById(id).subscribe({
      next: (response) => {
        if (response.data) {
          // Convert numeric ID to string for select component if needed
          const dataToPatch = {
            ...response.data,
            policyHolderId: response.data.policyHolderId.toString(),
          };
          this.form.patchValue(dataToPatch);
        }
        this.isLoading = false;
      },
      error: () => {
        this.globalErrorMessage = 'Failed to load policy details.';
        this.isLoading = false;
      },
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.form.get(fieldName);
    if (control?.invalid && (control.dirty || control.touched)) {
      if (control.hasError('required')) return 'This field is required.';
      if (control.hasError('min')) return 'Must be greater than zero.';
      if (control.hasError('max')) return 'Value exceeds the maximum allowed.';
    }
    return '';
  }

  onCancel(): void {
    if (this.isEditMode) {
      this.router.navigate(['/policies', this.policyId]);
    } else {
      this.router.navigate(['/policies']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.globalErrorMessage = '';
    const request = this.form.getRawValue();

    // Ensure numbers are numbers
    request.policyHolderId = +request.policyHolderId;
    request.coverageAmount = +request.coverageAmount;
    request.premiumAmount = +request.premiumAmount;

    const requestObservable =
      this.isEditMode && this.policyId
        ? this.service.updatePolicy(this.policyId, request)
        : this.service.createPolicy(request);

    requestObservable.subscribe({
      next: (response) => {
        this.isSaving = false;
        if (response.data) {
          this.router.navigate(['/policies', response.data.id]);
        } else {
          this.router.navigate(['/policies']);
        }
      },
      error: (err) => {
        this.isSaving = false;
        this.globalErrorMessage = err.error?.errors?.map(
          (e: any) => e.message
        ) || ['An error occurred while saving.'];
      },
    });
  }
}
