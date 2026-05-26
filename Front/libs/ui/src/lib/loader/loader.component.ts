import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css',
})
export class LoaderComponent {
  // If true, the loader takes up the full screen with a semi-transparent backdrop
  @Input() global: boolean = false;

  // Optional text to display next to or below the spinner
  @Input() text: string = '';
}
