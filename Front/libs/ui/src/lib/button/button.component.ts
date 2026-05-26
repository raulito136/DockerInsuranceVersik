import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type ButtonType = 'default' | 'confirm' | 'cancel' | 'submit' | 'custom';

@Component({
  selector: 'lib-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  @Input() type: ButtonType = 'default';
  @Input() content: string = '';
  @Input() disabled: boolean = false;

  // Output for handling the interaction with the button
  @Output() buttonClick = new EventEmitter<void>();

  baseClasses =
    'px-5 py-2 rounded-lg font-bold shadow-md transition-all duration-150 hover:-translate-y-1 hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-md';

  // Set color depending on the button type
  get colorClasses(): string {
    switch (this.type) {
      case 'default':
        return 'bg-gray-300 hover:bg-gray-400';

      case 'confirm':
        return 'bg-lime-400 hover:bg-lime-500';

      case 'cancel':
        return 'bg-red-500 hover:bg-red-600';

      case 'submit':
        return 'bg-cyan-300 hover:bg-cyan-400';

      default:
        return 'bg-gray-300 hover:bg-gray-400';
    }
  }

  // Button text content
  get contentText(): string {
    if (this.content) return this.content;

    switch (this.type) {
      case 'default':
        return 'Continue';

      case 'confirm':
        return 'Confirm';

      case 'cancel':
        return 'Cancel';

      case 'submit':
        return 'Submit';

      case 'custom':
        return 'Custom Button';

      default:
        return 'Continue';
    }
  }
}
