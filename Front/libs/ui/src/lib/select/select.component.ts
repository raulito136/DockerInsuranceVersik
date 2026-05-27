import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface OptionType {
  value: string | number;
  content: string;
}

@Component({
  selector: 'lib-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './select.component.html',
  styleUrl: './select.component.css',
})
export class SelectComponent {
  @Input() label: string = '';
  @Input() name: string = '';
  @Input() id: string =
    'ui-select-' + Math.random().toString(36).substring(2, 9);

  // Array of options
  @Input() options: OptionType[] = [];

  // Selection menu value that will return
  @Input() selectedOption: string | number = '';

  // Event for when the value has been changed
  @Output() selectedOptionChange = new EventEmitter<string | number>();

  // Get the native selection change event
  onSelectChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedOptionChange.emit(selectElement.value);
  }
}
