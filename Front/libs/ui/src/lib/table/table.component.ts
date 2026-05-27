import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TableColumn {
  key: string;
  label: string;
}

@Component({
  selector: 'lib-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css',
})
export class TableComponent {
  // Columns definition array
  @Input() columns: TableColumn[] = [];

  // Data rows to be rendered
  @Input() data: any[] = [];

  // Emit the selected row data when a row is clicked
  @Output() rowClick = new EventEmitter<any>();

  // Function to handle row click
  onRowClick(row: any) {
    this.rowClick.emit(row);
  }
}
