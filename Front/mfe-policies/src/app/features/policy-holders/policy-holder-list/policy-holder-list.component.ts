import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PolicyHolderService } from '../../../services/policy-holder.service';
import { PolicyHolder } from '../../../models/policy-holder.model';
import {
  TableComponent,
  PaginationComponent,
  ButtonComponent,
  LoaderComponent,
  InlineErrorComponent,
} from '@policy-system/ui';

@Component({
  selector: 'app-policy-holder-list',
  standalone: true,
  imports: [
    CommonModule,
    TableComponent,
    PaginationComponent,
    ButtonComponent,
    LoaderComponent,
    InlineErrorComponent,
  ],
  templateUrl: './policy-holder-list.component.html',
  styleUrl: './policy-holder-list.component.css',
})
export class PolicyHolderListComponent implements OnInit {
  private service = inject(PolicyHolderService);
  private router = inject(Router);

  policyHolders: PolicyHolder[] = [];
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  isLoading = false;
  errorMessage: string = '';

  columns = [
    { key: 'id', label: 'ID' },
    { key: 'firstName', label: 'First Name' },
    { key: 'lastName', label: 'Last Name' },
    { key: 'email', label: 'Email' },
    { key: 'regionCode', label: 'Region' },
  ];

  ngOnInit(): void {
    this.loadPolicyHolders();
  }

  loadPolicyHolders(page: number = 1): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.service.getPolicyHolders(page, this.pageSize).subscribe({
      next: (response) => {
        this.policyHolders = response.data;
        this.currentPage = response.page;
        this.totalPages = Math.ceil(response.total / response.pageSize) || 1;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.errors?.map((e: any) => e.message) || [
          'Failed to load policy holders.',
        ];
        this.isLoading = false;
      },
    });
  }

  onPageChange(page: number): void {
    this.loadPolicyHolders(page);
  }

  onRowSelect(row: any): void {
    this.router.navigate(['/policy-holders', row.id]);
  }

  onCreate(): void {
    this.router.navigate(['/policy-holders/new']);
  }
}
