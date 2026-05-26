import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-datepicker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './datepicker.component.html',
  styleUrl: './datepicker.component.css',
})
export class DatepickerComponent {
  @Input() label: string = '';
  @Input() name: string = '';
  @Input() id: string = 'ui-date-' + Math.random().toString(36).substring(2, 9);

  // Date value. HTML always uses 'YYYY-MM-DD'
  @Input() selectedDate: string = '';

  // Date limits (for insurance validation)
  @Input() minDate: string = '';
  @Input() maxDate: string = '';

  // Event for data changed
  @Output() selectedDateChange = new EventEmitter<string>();

  onDateChange(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    this.selectedDateChange.emit(inputElement.value);
  }
}
