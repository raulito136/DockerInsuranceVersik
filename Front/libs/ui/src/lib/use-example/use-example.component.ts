import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimelineComponent, TimelineEvent } from '../timeline/timeline.component';
import { ButtonComponent } from '../button/button.component';
import { TabsComponent } from '../tabs/tabs.component';
import { InputComponent } from '../input/input.component';
import { SelectComponent } from '../select/select.component';
import { DatepickerComponent } from '../datepicker/datepicker.component';
import { SwitchComponent } from '../switch/switch.component';
import { FormComponent } from '../form/form.component';
import { TableComponent } from '../table/table.component';
import { PaginationComponent } from '../pagination/pagination.component';
import { CardComponent } from '../card/card.component';
import { InlineErrorComponent } from '../inline-error/inline-error.component';
import { LoaderComponent } from '../loader/loader.component';

@Component({
  selector: 'lib-use-example',
  standalone: true,
  imports: [CommonModule, ButtonComponent, TabsComponent, InputComponent, SelectComponent, DatepickerComponent, SwitchComponent, FormComponent, TableComponent, PaginationComponent, CardComponent, TimelineComponent, InlineErrorComponent, LoaderComponent],
  templateUrl: './use-example.component.html',
  styleUrl: './use-example.component.css',
})
export class UseExampleComponent {
  // Tabs
  currentTab: string = 'Details';

  // Select
  carsOptions = [
    { value: 'bwm', content: 'BMW' },
    { value: 'nissan', content: 'Nissan' },
    { value: 'peugeot', content: 'Peugeot' },
    { value: 'alpha-romeo', content: 'Alpha Romeo' },
    { value: 'ferrari', content: 'Ferrari' },
  ];
  selectedCar: string | number = '';

  // Datepicker
  claimDate: string = '';
  today: string = new Date().toISOString().split('T')[0];

  // Switch
  isActive: boolean = false;

  // Input & Error
  firstName: string = '';
  lastName: string = '';
  exampleError: string = 'This field is required and must contain valid data.';

  // Table
  testTableCols = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Name' },
    { key: 'role', label: 'Role' },
    { key: 'status', label: 'Status' },
  ];
  testTableData = [
    { id: 1, name: 'Alice Smith', role: 'Admin', status: 'Active' },
    { id: 2, name: 'Bob Johnson', role: 'User', status: 'Inactive' },
    { id: 3, name: 'Charlie Davis', role: 'Manager', status: 'Active' },
  ];

  onRowSelect(row: any) {
    console.log('Row selected:', row);
  }

  // Pagination
  currentPage: number = 1;

  // Card
  isCardLoading: boolean = false;

  // Timeline
  testTimelineEvents: TimelineEvent[] = [
    { title: 'Claim Drafted', timestamp: 'Yesterday, 10:00 AM', description: 'Agent started drafting the claim.', status: 'default' },
    { title: 'Information Required', timestamp: 'Yesterday, 02:30 PM', description: 'Additional documents requested from customer.', status: 'warning' },
    { title: 'Documents Uploaded', timestamp: 'Today, 09:15 AM', status: 'info' },
    { title: 'Claim Approved', timestamp: 'Today, 11:45 AM', description: 'Claim successfully processed and approved.', status: 'success' },
  ];

  // Loader
  showGlobalLoader: boolean = false;

  triggerGlobalLoader() {
    this.showGlobalLoader = true;
    setTimeout(() => {
      this.showGlobalLoader = false;
    }, 2500);
  }
}
