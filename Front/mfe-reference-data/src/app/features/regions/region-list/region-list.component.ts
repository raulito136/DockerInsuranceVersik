import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegionService } from 'mfe-reference-data/src/app/services/region.service';
import { Region } from 'mfe-reference-data/src/app/models/region.model';
import { RouterLink } from '@angular/router';
import { SwitchComponent } from 'libs/ui/src/lib/switch/switch.component';
import { LoaderComponent } from 'libs/ui/src/lib/loader/loader.component';
import { ButtonComponent } from 'libs/ui/src/lib/button/button.component';
import { ConfirmationComponent } from 'libs/ui/src/lib/confirmation/confirmation.component';

@Component({
    selector: 'app-region-list',
    imports: [CommonModule, RouterLink, SwitchComponent, LoaderComponent, ButtonComponent, ConfirmationComponent],
    templateUrl: './region-list.component.html',
    styleUrl: './region-list.component.css'})
export class RegionListComponent {
  private status=false;
  private regionService=inject(RegionService);
  regions = signal<Region[]>([]);
  loading = signal<boolean>(true);

  public errores= signal<string[]>([]);
  public showAll = signal<boolean>(false);

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
    this.regionService.getRegions(this.showAll()).subscribe({
      next:(apiResponse)=>{
        this.regions.set(apiResponse.data);
        this.loading.set(false);
      },
      error:(err)=>{
        console.error('Error fetching regions:', err);
        this.errores.set([err]);
        this.loading.set(false);
      }
    })
  }

  async delete(id:number){
    const confirmationValue = await this.askForConfirmation(
      'Confirm Delete',
      'Are you sure you want to delete this Region?'
    );


  if (!confirmationValue) return;
    this.errores.set([]);
    this.regionService.deleteRegion(id).subscribe({
      next:()=>{
        console.log('Region deleted successfully');
        this.loadAll();
      },
      error:(err)=>{
        console.error('Error deleting region:', err);
        this.errores.set([err]);
      }
    })
  }
  
  onShowAllChange(value: boolean) {
    console.log('Switch cambiado a:', value, 'haciendo llamada a la API...');
    this.showAll.set(value);
    this.loadAll();
  }

  toggle(policy: Region) {
          if (policy.isActive) {
            this.delete(policy.id);
          } else {
            this.errores.set([]);
            this.regionService.activateRegion(policy.id).subscribe({
              next: (apiResponse) => {
                console.log("Region activated successfully:", apiResponse);
                this.loadAll();
              },
              error: (err) => {
                console.error("Error activating region:", err);
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