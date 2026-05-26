import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-form',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './form.component.html',
  styleUrl: './form.component.css',
})
export class FormComponent {
  // Optional title for the form
  @Input() title: string = '';

  // Optional subtitle for the form
  @Input() subtitle: string = '';

  // Event emitted when the form is submitted
  @Output() formSubmit = new EventEmitter<Event>();

  // Event handler for form submission
  onSubmit(event: Event) {
    event.preventDefault(); // Prevent default browser form submission
    this.formSubmit.emit(event);
  }
}
