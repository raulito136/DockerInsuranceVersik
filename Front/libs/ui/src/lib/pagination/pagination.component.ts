import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css',
})
export class PaginationComponent {
  // Current active page (1-indexed)
  @Input() currentPage: number = 1;

  // Total number of pages
  @Input() totalPages: number = 1;

  // Emits the new page number when it changes
  @Output() pageChange = new EventEmitter<number>();

  // Go to previous page
  previousPage() {
    if (this.currentPage > 1) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  // Go to next page
  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }
}
