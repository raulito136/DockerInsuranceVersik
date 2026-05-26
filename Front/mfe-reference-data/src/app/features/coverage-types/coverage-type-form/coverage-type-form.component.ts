import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoverageTypeService } from 'mfe-reference-data/src/app/services/coverage-type.service';
import { CoverageType } from 'mfe-reference-data/src/app/models/coverage-type.model';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { TextareaComponent } from 'libs/ui/src/lib/textarea/textarea.component';
import { InputComponent } from 'libs/ui/src/lib/input/input.component';
import { FormComponent } from 'libs/ui/src/lib/form/form.component';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';


@Component({
    selector: 'app-coverage-type-form',
    imports: [CommonModule, FormComponent, InputComponent, TextareaComponent, ButtonComponent, SwitchComponent, ReactiveFormsModule],
    templateUrl: './coverage-type-form.component.html',
    styleUrl: './coverage-type-form.component.css'
})
export class CoverageTypeFormComponent implements OnInit {
    private coverageService= inject(CoverageTypeService);
    private fb = inject(FormBuilder);
    private router= inject(Router);
    private route = inject(ActivatedRoute);

    public isEditMode = false;

    public myForm= this.fb.group({
      id: [null as number | null],
      code:['',[Validators.required]],
      name:['',[Validators.required]],
      description:[''],
      isActive:[true]
    });

  ngOnInit(): void {
    const id= this.route.snapshot.paramMap.get('id');
    if(id){
      this.isEditMode=true;
      this.myForm.get('code')?.clearValidators();
      this.myForm.get('code')?.updateValueAndValidity();

      this.coverageService.getCoverageTypeById(Number(id)).subscribe({
        next: (res) =>{
          if(res.data){
            this.myForm.patchValue({
              id: res.data.id,
              code: res.data.code,
              name: res.data.name,
              description: res.data.description,
              isActive: res.data.isActive
            });
          }
        },
        error: (err) => console.error('Error fetching coverage type:', err)
    });
  }
}
  onSubmitForm(event:Event){
    if(this.myForm.invalid) return;

    const formValue = this.myForm.value;

    if(!this.isEditMode){
      const  createData={
        code: formValue.code ?? '',
        name: formValue.name ?? '',
        description: formValue.description ?? ''
      };

      this.coverageService.createCoverageType(createData as any).subscribe({
        next:(res)=>{
          console.log("Creado: ", res);
          this.router.navigate(['../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error creating coverage type:', err);
        }
      });
    }else{
      const updateData={
        name: formValue.name ?? '',
        description: formValue.description ?? '',
        isActive: formValue.isActive ?? true
      }

      this.coverageService.updateCoverageType(formValue.id!, updateData as any).subscribe({
        next:(res)=>{
          console.log("Actualizado: ", res);
          this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error updating coverage type:', err);
        }
      });
    }
  }

    onCancel() {
    const path = this.isEditMode ? '../../' : '../';
    this.router.navigate([path], { relativeTo: this.route });
  }

}
