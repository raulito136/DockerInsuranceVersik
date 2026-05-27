import { Component, inject, signal } from '@angular/core';
import { CommonModule, formatNumber } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ClaimService } from '../../../services/claim.service';
import { FormComponent, InputComponent, ButtonComponent, DatepickerComponent, ConfirmationComponent } from '@policy-system/ui';

@Component({
  selector: 'app-claim-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormComponent, InputComponent, ButtonComponent, DatepickerComponent, ConfirmationComponent],
  templateUrl: './claim-form.component.html'
})
export class ClaimFormComponent {
  private fb = inject(FormBuilder);
  private claimService = inject(ClaimService);
  private router = inject(Router);

  showModal = signal(false);
  modalTitle = signal('');
  modalText = signal('');

  claimForm = this.fb.group({
    policyNumber: ['', Validators.required],
    claimDate: ['', Validators.required],
    amount: ['', [Validators.required, this.amountValidator]],
    description: ['', Validators.required]
  });

  onAmountChange(val: string) {
    if (!val) {
      this.claimForm.get('amount')?.setValue('');
      return;
    }

    let cleanValue = val.toString().replace(/\D/g, '');
    if (cleanValue.length > 16) {
      cleanValue = cleanValue.substring(0, 16);
    }
    const formatted = cleanValue ? cleanValue.replace(/\B(?=(\d{3})+(?!\d))/g, ".") : '';

    if (val !== formatted) {
      this.claimForm.get('amount')?.setValue(val, { emitEvent: false });
      setTimeout(() => {
        this.claimForm.get('amount')?.setValue(formatted);
      });
    } else {
      this.claimForm.get('amount')?.setValue(formatted);
    }
  }

  onAmountKeydown(event: KeyboardEvent) {
    const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Home', 'End'];

    if (allowedKeys.includes(event.key) || event.ctrlKey || event.metaKey) {
      return;
    }

    if (!/^\d$/.test(event.key)) {
      event.preventDefault();
      return;
    }

    const currentVal = this.claimForm.get('amount')?.value || '';
    const digits = currentVal.toString().replace(/\D/g, '');

    if (digits.length >= 16) {
      const inputElement = event.target as HTMLInputElement;
      if (inputElement && inputElement.selectionStart !== null && inputElement.selectionEnd !== null) {
        if (inputElement.selectionStart !== inputElement.selectionEnd) {
          return;
        }
      }
      event.preventDefault();
    }
  }

  amountValidator(control: AbstractControl) {
    if (!control.value) return null;
    const cleanValue = control.value.toString().replace(/\./g, '');
    const num = Number(cleanValue);
    if (num < 1) return { min: true };
    if (num > 9999999999999999) return { max: true };
    return null;
  }

  guardarDatos() {
    if (this.claimForm.valid) {
      const amountStr = this.claimForm.value.amount ? this.claimForm.value.amount.toString().replace(/\./g, '') : '0';
      const data = {
        ...this.claimForm.value,
        amount: Number(amountStr)
      };

      this.claimService.createClaim(data).subscribe({
        next: () => this.router.navigate(['claims']),
        error: (err) => {
          let detailedMessage = '';

          if (err.error?.errors) {
            if (Array.isArray(err.error.errors)) {
              detailedMessage = err.error.errors
                .map((e: any) => e.message || e.Message || 'Validation error')
                .join(', ');
            } else if (typeof err.error.errors === 'object') {
              detailedMessage = Object.values(err.error.errors).flat().join(', ');
            }
          } else if (err.error?.message) {
            detailedMessage = err.error.message;
          } else if (typeof err.error === 'string') {
            detailedMessage = err.error;
          }

          if (detailedMessage.toLowerCase().includes('too large') ||
            detailedMessage.toLowerCase().includes('int64') ||
            detailedMessage.toLowerCase().includes('overflow')) {
            detailedMessage = 'Internal system error: Value too large for target type (long/decimal).';
          }

          console.error('Registration failed:', detailedMessage || err);
          this.showAlert('Registration Error', 'Please fill all required fields correctly.');
        }
      });
    } else {
      let detailedError = 'Form is invalid.';
      const amountControl = this.claimForm.get('amount');

      if (amountControl?.errors?.['max']) {
        detailedError = 'Validation: Amount exceeds maximum allowed limit ($999,999,999.99).';
      } else if (amountControl?.errors?.['min']) {
        detailedError = 'Validation: Amount must be greater than 0.';
      }

      console.warn('Validation failed:', detailedError);
      this.showAlert('Registration Error', 'Please fill all required fields correctly.');
    }
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
    this.router.navigate(['claims']);
  }
}