import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PolicyService } from '../../../services/policy.service';
import { ReferenceDataService } from '../../../services/reference-data.service';
import { Policy } from '../../../models/policy.model';
import {
  TableComponent,
  PaginationComponent,
  ButtonComponent,
  LoaderComponent,
  SelectComponent,
  InlineErrorComponent,
} from '@policy-system/ui';

@Component({
  selector: 'app-policy-list',
  standalone: true,
  imports: [
    CommonModule,
    TableComponent,
    PaginationComponent,
    ButtonComponent,
    LoaderComponent,
    SelectComponent,
    InlineErrorComponent,
  ],
  templateUrl: './policy-list.component.html',
  styleUrl: './policy-list.component.css',
})
export class PolicyListComponent implements OnInit {
  private service = inject(PolicyService);
  private refDataService = inject(ReferenceDataService);
  private router = inject(Router);

  policies: Policy[] = [];
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  isLoading = false;
  errorMessage: string = '';

  // Filters
  selectedStatus = 'ALL';
  selectedPolicyTypeCode = 'ALL';

  statusOptions = [
    { value: 'ALL', content: 'All Statuses' },
    { value: 'ACTIVE', content: 'Active' },
    { value: 'INACTIVE', content: 'Inactive' },
    { value: 'EXPIRED', content: 'Expired' },
    { value: 'CANCELLED', content: 'Cancelled' },
  ];

  policyTypeOptions: { value: string; content: string }[] = [
    { value: 'ALL', content: 'All Types' },
  ];

  columns = [
    { key: 'policyNumber', label: 'Policy Number' },
    { key: 'policyTypeCode', label: 'Type' },
    { key: 'coverageTypeCode', label: 'Coverage' },
    { key: 'startDate', label: 'Start Date' },
    { key: 'endDate', label: 'End Date' },
    { key: 'status', label: 'Status' },
  ];

  ngOnInit(): void {
    this.loadPolicyTypes();
    this.loadPolicies();
  }

  loadPolicyTypes(): void {
    this.refDataService.getPolicyTypes().subscribe((types) => {
      this.policyTypeOptions = [
        { value: 'ALL', content: 'All Types' },
        ...types.map((t) => ({ value: t.code, content: t.name })),
      ];
    });
  }

  loadPolicies(page: number = 1): void {
    this.isLoading = true;
    this.errorMessage = '';

    const statusParam =
      this.selectedStatus === 'ALL' || this.selectedStatus === ''
        ? undefined
        : this.selectedStatus;
    const typeParam =
      this.selectedPolicyTypeCode === 'ALL' ||
      this.selectedPolicyTypeCode === ''
        ? undefined
        : this.selectedPolicyTypeCode;

    this.service
      .getPolicies(page, this.pageSize, statusParam, typeParam)
      .subscribe({
        next: (response) => {
          this.policies = response.data;
          this.currentPage = response.page;
          this.totalPages = Math.ceil(response.total / response.pageSize) || 1;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.errors?.map((e: any) => e.message) || [
            'Failed to load policies.',
          ];
          this.isLoading = false;
        },
      });
  }

  onPageChange(page: number): void {
    this.loadPolicies(page);
  }

  onFilterChange(): void {
    this.loadPolicies(1);
  }

  onRowSelect(row: any): void {
    this.router.navigate(['/policies', row.id]);
  }

  goToPolicyCreation(): void {
    this.router.navigate(['/policies/new']);
  }
}
