import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-inline-error',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inline-error.component.html',
  styleUrl: './inline-error.component.css',
})
export class InlineErrorComponent {
  // The error message to display. If empty, the component won't render anything.
  @Input() message: string = '';
}
