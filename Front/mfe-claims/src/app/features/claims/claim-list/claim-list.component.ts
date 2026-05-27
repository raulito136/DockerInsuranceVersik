import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, formatNumber } from '@angular/common';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { ClaimService } from '../../../services/claim.service';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ReferenceDataService } from '../../../services/reference-data.service';
import { TableComponent, ButtonComponent, LoaderComponent, InputComponent, SelectComponent } from '@policy-system/ui';

@Component({
  selector: 'app-claim-list',
  imports: [
    CommonModule, 
    RouterModule, 
    ReactiveFormsModule,
    TableComponent, 
    ButtonComponent, 
    LoaderComponent, 
    InputComponent, 
    SelectComponent
  ],
  templateUrl: './claim-list.component.html'
})
export class ClaimListComponent implements OnInit {
  private claimService = inject(ClaimService);
  private refDataService = inject(ReferenceDataService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  claimsList = signal<any[]>([]);
  loading = signal<boolean>(true);
  statusOptions: { value: string, content: string }[] = [];
  
  filterForm: FormGroup = this.fb.group({
    policyNumber: [''],
    statusCode: ['']
  });

  tableColumns = [
    { key: 'claimNumber', label: 'Claim Number' },
    { key: 'policyNumber', label: 'Policy' },
    { key: 'statusCode', label: 'Status' },
    { key: 'formattedAmount', label: 'Amount' },
    { key: 'claimDate', label: 'Date' }
  ];

  ngOnInit() {
    this.loadStatuses();
    this.loadClaims();
    
    this.filterForm.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.loadClaims();
    });
  }

  loadStatuses() {
    this.refDataService.getClaimStatuses().subscribe(statuses => {
      this.statusOptions = [
        { value: '', content: 'All Statuses' },
        ...statuses.map(s => ({ value: s.code, content: s.name }))
      ];
    });
  }

  loadClaims() {
    this.loading.set(true);
    const filters = this.filterForm.value;
    this.claimService.getAllClaims(1, 20, filters.statusCode, filters.policyNumber).subscribe(res => {
      const formattedData = res.data.map((claim: any) => ({
        ...claim,
        formattedAmount: claim.amount != null ? '$' + formatNumber(claim.amount, 'es-ES', '1.0-0') : ''
      }));
      this.claimsList.set(formattedData);
      this.loading.set(false);
    });
  }

  applyFilters() {
    this.loadClaims();
  }

  clearFilters() {
    this.filterForm.reset();
    this.loadClaims();
  }

  irAlDetalle(fila: any) {
    this.router.navigate([fila.id], { relativeTo: this.route });
  }
}