import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ClaimStatusService } from 'mfe-reference-data/src/app/services/claim-status.service';
import { FormComponent } from 'libs/ui/src/lib/form/form.component';
import { InputComponent } from 'libs/ui/src/lib/input/input.component';
import { TextareaComponent } from 'libs/ui/src/lib/textarea/textarea.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { ClaimStatus } from 'mfe-reference-data/src/app/models/claim-status.model';

@Component({
  selector: 'app-claim-status-form',
  standalone: true,
  imports: [CommonModule, FormComponent, InputComponent, TextareaComponent, ButtonComponent, SwitchComponent, ReactiveFormsModule], 
  templateUrl: './claim-status-form.component.html',
  styleUrl: './claim-status-form.component.css'
})
export class ClaimStatusFormComponent implements OnInit {
  private claimService = inject(ClaimStatusService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  public isEditMode = false;

  public myForm = this.fb.group({
    id: [null as number | null], 
    code: ['', [Validators.required]],
    name: ['', [Validators.required]],
    description: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      
      this.isEditMode = true;
      this.myForm.get('code')?.clearValidators();
      this.myForm.get('code')?.updateValueAndValidity();

      this.claimService.getClaimStatusById(Number(id)).subscribe({
        next: (res) => {
          if (res.data) {
            this.myForm.patchValue({
              id: res.data.id,
              code: res.data.code,
              name: res.data.name,
              description: res.data.description,
              isActive: res.data.isActive ?? true
            });
          }
        },
        error: (err) => console.error('Error fetching claim status:', err)
      });
    }
  }

  onCancel() {
    const path = this.isEditMode ? '../../' : '../';
    this.router.navigate([path], { relativeTo: this.route });
  }

  onSubmitForm(event: Event) {
  if (this.myForm.invalid) return;

  const formValues = this.myForm.getRawValue();

  if (!this.isEditMode) {
    const createPayload = {
      code: formValues.code ?? '',
      name: formValues.name ?? '',
      description: formValues.description ?? ''
    };

    this.claimService.createClaimStatus(createPayload as any).subscribe({
      next: (res) => {
        console.log('Creado:', res);
    this.router.navigate(['../'], { relativeTo: this.route });
      },
      error: (err) => console.error(err)
    });

  } else {
    const updatePayload = {
      name: formValues.name ?? '',
      description: formValues.description ?? '',
      isActive: formValues.isActive ?? true
    };

    this.claimService.updateClaimStatus(formValues.id!, updatePayload as any).subscribe({
      next: (res) => {
        console.log('Actualizado:', res);
        this.router.navigate(['../../'], { relativeTo: this.route });
      },
      error: (err) => console.error(err)
    });
  }
}
}