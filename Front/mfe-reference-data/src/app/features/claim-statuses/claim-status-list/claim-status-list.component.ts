import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClaimStatusService } from 'mfe-reference-data/src/app/services/claim-status.service';
import { ClaimStatus } from 'mfe-reference-data/src/app/models/claim-status.model';
import { RouterLink } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { LoaderComponent } from 'libs/ui/src/lib/loader/loader.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { ConfirmationComponent } from 'libs/ui/src/lib/confirmation/confirmation.component';

@Component({
    selector: 'app-claim-status-list',
    imports: [CommonModule, RouterLink, SwitchComponent, LoaderComponent, ButtonComponent, LoaderComponent, ConfirmationComponent],
    templateUrl: './claim-status-list.component.html',
    styleUrl: './claim-status-list.component.css'
})
export class ClaimStatusListComponent {
  errores = signal<string[]>([]);
  showAll = signal<boolean>(false);
  
  private claimService = inject(ClaimStatusService);

  claimsStatuses = signal<ClaimStatus[]>([]);
  loading = signal<boolean>(true);

  title: string = '';
  text: string = '';
  isConfirming = false;
  private resolveConfirmation: ((value: boolean) => void) | null = null;

  ngOnInit(){
    this.loadAll();
  }

  loadAll(){
    this.loading.set(true);
    this.errores.set([]);
    this.claimService.getClaimStatuses(this.showAll()).subscribe({
      next:(apiResponse)=>{
        this.claimsStatuses.set(apiResponse.data);
        this.loading.set(false);
      },
      error: (err)=>{
        this.errores.set([err]);
        this.loading.set(false);
      }
    })
  }

  onShowAllChange(value: boolean) {
    console.log('Switch cambiado a:', value, 'haciendo llamada a la API...');
    this.showAll.set(value);
    this.loadAll();
  }

  async delete(id:number){
const confirmationValue = await this.askForConfirmation(
      'Confirm Delete',
      'Are you sure you want to delete this claim status?'
    );
  if (!confirmationValue) return;
    this.errores.set([]);
    this.claimService.deleteClaimStatus(id).subscribe({
      next:(apiResponse)=>{
        this.loadAll();
      },
      error: (err)=>{
        this.errores.set([err]);
      }
    })
  }

  toggle(policy: ClaimStatus) {
      if (policy.isActive) {
        this.delete(policy.id);
      } else {
        this.errores.set([]);
        this.claimService.activateClaimStatus(policy.id).subscribe({
          next: (apiResponse) => {
            console.log("Claim Status activated successfully:", apiResponse);
            this.loadAll();
          },
          error: (err) => {
            console.error("Error activating claim status:", err);
            this.errores.set([err]);
          }
        });
      }
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