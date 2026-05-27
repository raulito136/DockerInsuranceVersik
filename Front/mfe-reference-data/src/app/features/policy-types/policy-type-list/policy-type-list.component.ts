import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { PolicyType } from 'mfe-reference-data/src/app/models/policy-type.model';
import { PolicyTypeService } from 'mfe-reference-data/src/app/services/policy-type.service';
import { RouterLink } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { LoaderComponent } from 'libs/ui/src/lib/loader/loader.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { ConfirmationComponent } from 'libs/ui/src/lib/confirmation/confirmation.component';

@Component({
    selector: 'app-policy-type-list',
    imports: [CommonModule, RouterLink, SwitchComponent, LoaderComponent, ButtonComponent, ConfirmationComponent],
    templateUrl: './policy-type-list.component.html',
    styleUrl: './policy-type-list.component.css'
})
export class PolicyTypeListComponent {
  private status=false;
  private policyService=inject(PolicyTypeService);
  policyTypes = signal<PolicyType[]>([]);
  loading = signal<boolean>(true);
  public showAll = signal<boolean>(false);

  public errores = signal<string[]>([]);

  title: string = '';
  text: string = '';
  isConfirming = false;
  private resolveConfirmation: ((value: boolean) => void) | null = null;



  ngOnInit(){
    this.loadAll();
  }

  loadAll(){
    this.errores.set([]);
    this.loading.set(true);
    this.policyService.getPolicyTypes(this.showAll()).subscribe({
      next:(apiResponse)=>{
        this.policyTypes.set(apiResponse.data);
        this.loading.set(false);
      },
      error:(err)=>{
        console.error('Error fetching policy types:', err);
        this.errores.set([err]);
        this.loading.set(false);
      }
    })
  }

  async delete(id: number) {
  const confirmationValue = await this.askForConfirmation(
      'Confirm Delete',
      'Are you sure you want to delete this Policy Type?'
    );
  if (!confirmationValue) return;

  this.errores.set([]);
  this.policyService.deletePolicyType(id).subscribe({
    next: (apiResponse) => {
      console.log("Policy Type deleted successfully:", apiResponse);
      this.loadAll();
    },
    error: (err) => {
      console.error("Error deleting policy type:", err);
      this.errores.set([err]);
    }
  });

}

  toggle(policy: PolicyType) {
    if (policy.isActive) {
      this.delete(policy.id);
    } else {
      this.errores.set([]);
      this.policyService.activatePolicyType(policy.id).subscribe({
        next: (apiResponse) => {
          console.log("Policy Type activated successfully:", apiResponse);
          this.loadAll();
        },
        error: (err) => {
          console.error("Error activating policy type:", err);
          this.errores.set([err]);
        }
      });
    }
  }

  onShowAllChange(value: boolean) {
    console.log('Switch cambiado a:', value, 'haciendo llamada a la API...');
    this.showAll.set(value);
    this.loadAll();
  }

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