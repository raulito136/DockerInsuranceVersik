import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PolicyTypeService } from 'mfe-reference-data/src/app/services/policy-type.service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { TextareaComponent } from 'libs/ui/src/lib/textarea/textarea.component';
import { InputComponent } from 'libs/ui/src/lib/input/input.component';
import { FormComponent } from 'libs/ui/src/lib/form/form.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-policy-type-form',
    imports: [CommonModule, FormComponent, InputComponent, TextareaComponent, ButtonComponent, SwitchComponent, ReactiveFormsModule],
    templateUrl: './policy-type-form.component.html',
    styleUrl: './policy-type-form.component.css'
})
export class PolicyTypeFormComponent implements OnInit {
  private policyService= inject(PolicyTypeService);
  private fb= inject(FormBuilder);
  private router= inject(Router);
  private route = inject(ActivatedRoute);


  public isEditMode = false;

  public myForm= this.fb.group({
    id: [null as number | null],
    code:['', [Validators.required]],
    name:['', [Validators.required]],
    description:[''],
    isActive:[true]
  });

  ngOnInit(){
    const id= this.route.snapshot.paramMap.get('id');

    if(id){
      this.isEditMode=true;

      this.myForm.get('code')?.clearValidators();
      this.myForm.get('code')?.updateValueAndValidity();

      this.policyService.getPolicyTypeById(Number(id)).subscribe({
      next:(res)=>{
        if(res.data){
          this.myForm.patchValue({
            id: res.data.id,
            code: res.data.code,
            name: res.data.name,
            description: res.data.description,
            isActive: res.data.isActive
          })
        }
      },
      error:(err)=>{
          console.error('Error fetching policy type:', err);
        }
      });
    }
  }

  onSubmitForm(event:Event){
    if(this.myForm.invalid) return;

    const formValue = this.myForm.value;

    if(this.isEditMode){
      const updateData={
        name: formValue.name ?? '',
        description: formValue.description ?? '',
        isActive: formValue.isActive ?? true
      }
      
      this.policyService.updatePolicyType(formValue.id!, updateData as any).subscribe({
        next:(res)=>{
          console.log("Actualizado: ", res);
          this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error updating policy type:', err);

        }
      });
    }else{
      const createData={
        code: formValue.code ?? '',
        name: formValue.name ?? '',
        description: formValue.description ?? ''
      };

      this.policyService.createPolicyType(createData as any).subscribe({
        next:(res)=>{
          console.log("Creado: ", res);
          this.router.navigate(['../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error creating policy type:', err);
        }
      });
    }
  }

  onCancel() {
    const path = this.isEditMode ? '../../' : '../';
    this.router.navigate([path], { relativeTo: this.route });
  }


  
}
