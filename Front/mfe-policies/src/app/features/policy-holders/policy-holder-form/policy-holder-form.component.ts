import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyHolderService } from '../../../services/policy-holder.service';
import { ReferenceDataService } from '../../../services/reference-data.service';
import { FormComponent, InputComponent, SelectComponent, DatepickerComponent, ButtonComponent, LoaderComponent, InlineErrorComponent } from '@policy-system/ui';

@Component({
  selector: 'app-policy-holder-form',
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
    InlineErrorComponent
  ],
  templateUrl: './policy-holder-form.component.html',
  styleUrl: './policy-holder-form.component.css',
})
export class PolicyHolderFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(PolicyHolderService);
  private refDataService = inject(ReferenceDataService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  form!: FormGroup;
  isEditMode = false;
  policyHolderId?: number;
  isLoading = false;
  isSaving = false;
  
  regionOptions: { value: string, content: string }[] = [];
  globalErrorMessages: string = '';

  ngOnInit(): void {
    this.initForm();
    this.loadRegions();
    
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam && idParam !== 'new') {
      this.isEditMode = true;
      this.policyHolderId = +idParam;
      this.loadPolicyHolder(this.policyHolderId);
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      phone: ['', [Validators.maxLength(20)]],
      dateOfBirth: ['', [Validators.required]],
      regionCode: ['', [Validators.required]]
    });
  }

  private loadRegions(): void {
    this.refDataService.getRegions().subscribe(regions => {
      this.regionOptions = regions.map(r => ({
        value: r.code,
        content: `${r.name} (${r.code})`
      }));
    });
  }

  private loadPolicyHolder(id: number): void {
    this.isLoading = true;
    this.service.getPolicyHolderById(id).subscribe({
      next: (response) => {
        if (response.data) {
          this.form.patchValue(response.data);
        }
        this.isLoading = false;
      },
      error: () => {
        this.globalErrorMessages = 'Failed to load policy holder details.';
        this.isLoading = false;
      }
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.form.get(fieldName);
    if (control?.invalid && (control.dirty || control.touched)) {
      if (control.hasError('required')) return 'This field is required.';
      if (control.hasError('email')) return 'Please enter a valid email address.';
      if (control.hasError('maxlength')) return 'Value is too long.';
    }
    return '';
  }

  onCancel(): void {
    if (this.isEditMode) {
      this.router.navigate(['/policy-holders', this.policyHolderId]);
    } else {
      this.router.navigate(['/policy-holders']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.globalErrorMessages = '';
    const request = this.form.value;

    const requestObservable = this.isEditMode && this.policyHolderId
      ? this.service.updatePolicyHolder(this.policyHolderId, request)
      : this.service.createPolicyHolder(request);

    requestObservable.subscribe({
      next: (response) => {
        this.isSaving = false;
        if (response.data) {
          this.router.navigate(['/policy-holders', response.data.id]);
        } else {
          this.router.navigate(['/policy-holders']);
        }
      },
      error: (err) => {
        this.isSaving = false;
        this.globalErrorMessages = err.error?.errors?.map((e: any) => e.message) || ['An error occurred while saving.'];
      }
    });
  }
}
