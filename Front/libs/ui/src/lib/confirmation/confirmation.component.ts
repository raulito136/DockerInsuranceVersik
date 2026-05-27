import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'lib-confirmation',
  standalone: true,
  imports: [CommonModule, ButtonComponent],
  templateUrl: './confirmation.component.html',
  styleUrl: './confirmation.component.css',
})
export class ConfirmationComponent {
  @Input() title: string = 'Title';
  @Input() text: string = 'Text';

  @Output() confirmAction = new EventEmitter<boolean>();

  onConfirm(): void {
    this.confirmAction.emit(true);
  }

  onCancel(): void {
    this.confirmAction.emit(false);
  }
}
