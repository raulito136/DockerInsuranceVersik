import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegionService } from 'mfe-reference-data/src/app/services/region.service';
import { Region } from 'mfe-reference-data/src/app/models/region.model';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { TextareaComponent } from 'libs/ui/src/lib/textarea/textarea.component';
import { InputComponent } from 'libs/ui/src/lib/input/input.component';
import { FormComponent } from 'libs/ui/src/lib/form/form.component';

@Component({
    selector: 'app-region-form',
    imports: [CommonModule, FormComponent, InputComponent, TextareaComponent, ButtonComponent, SwitchComponent, ReactiveFormsModule],
    templateUrl: './region-form.component.html',
    styleUrl: './region-form.component.css'
})
export class RegionFormComponent implements OnInit {
  private regionService=inject(RegionService);
  private fb= inject(FormBuilder);
  private router= inject(Router);
  private route= inject(ActivatedRoute);

  public isEditMode = false;

  public myForm= this.fb.group({
    id: [null as number | null],
    code:['', [Validators.required]],
    name:['', [Validators.required]],
    isActive:[true]
  });

  ngOnInit(){
    const id= this.route.snapshot.paramMap.get('id');
    if(id){
      this.isEditMode=true;

      this.regionService.getRegionsById(Number(id)).subscribe({
        next:(res)=>{
          if(res.data){
            this.myForm.patchValue({
              id: res.data.id,
              code: res.data.code,
              name: res.data.name,
              isActive: res.data.isActive
            })
          }
        },
        error:(err)=>{
          console.error('Error fetching region:', err);
        }
      });
    }
  }

  onSubmitForm(event:Event){
    if(this.myForm.invalid) return;

    const formValue= this.myForm.value;

    if(this.isEditMode){
      const updateData={
        code: formValue.code,
        name: formValue.name,
        isActive: formValue.isActive
      };
      this.regionService.updateRegion(formValue.id!, updateData as any).subscribe({
        next:(res)=>{
          console.log('Region updated successfully:', res);
          this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error updating region:', err);
        }
      })
    } else{
      const createData={
        code: formValue.code,
        name: formValue.name 
      };

      this.regionService.createRegion(createData as any).subscribe({
        next:(res)=>{
          console.log('Region created successfully:', res);
          this.router.navigate(['../'], { relativeTo: this.route });
        },
        error:(err)=>{
          console.error('Error creating region:', err);
        }
      });
  }
}
    onCancel() {
    const path = this.isEditMode ? '../../' : '../';
    this.router.navigate([path], { relativeTo: this.route });
  }

}
