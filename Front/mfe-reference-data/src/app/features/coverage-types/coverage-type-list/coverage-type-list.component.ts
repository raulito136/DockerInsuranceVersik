import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoverageTypeService } from 'mfe-reference-data/src/app/services/coverage-type.service';
import { CoverageType } from 'mfe-reference-data/src/app/models/coverage-type.model';
import { RouterLink } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { LoaderComponent } from 'libs/ui/src/lib/loader/loader.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { ConfirmationComponent } from 'libs/ui/src/lib/confirmation/confirmation.component';


@Component({
    selector: 'app-coverage-type-list',
    imports: [CommonModule, RouterLink, SwitchComponent, LoaderComponent, ButtonComponent, ConfirmationComponent],
    templateUrl: './coverage-type-list.component.html',
    styleUrl: './coverage-type-list.component.css'
})
export class CoverageTypeListComponent {
  private coverageService=inject(CoverageTypeService);
  coverageTypes = signal<CoverageType[]>([]);
  loading = signal<boolean>(true);

  errores = signal<string[]>([]);

  showAll = signal<boolean>(false);

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
    this.coverageService.getCoverageTypes(this.showAll()).subscribe({
      next:(apiResponse)=>{
        this.coverageTypes.set(apiResponse.data);
        this.loading.set(false);
      },
      error: (err)=>{
        this.errores.set([err]);
        console.error('Error fetching coverage types:', err);
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
      'Are you sure you want to delete this Coverage Type?'
    );

  if (!confirmationValue) return;
    this.errores.set([]);
    this.coverageService.deleteCoverageType(id).subscribe({
      next:(apiResponse)=>{
        console.log("Coverage Type deleted successfully:", apiResponse);
        this.loadAll();
      },
      error:(err)=>{
        this.errores.set([err]);
        console.error("Error deleting coverage type:", err);
      }
    })
  }

  toggle(policy: CoverageType) {
        if (policy.isActive) {
          this.delete(policy.id);
        } else {
          this.errores.set([]);
          this.coverageService.activateCoverageType(policy.id).subscribe({
            next: (apiResponse) => {
              console.log("Coverage Type activated successfully:", apiResponse);
              this.loadAll();
            },
            error: (err) => {
              console.error("Error activating coverage type:", err);
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