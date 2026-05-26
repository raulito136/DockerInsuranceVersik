import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lib-tabs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tabs.component.html',
  styleUrl: './tabs.component.css',
})
export class TabsComponent {
  // List of tabs name
  @Input() tabs: string[] = [];

  // Active tab by default
  @Input() activeTab: string = '';

  // Throw event with the selected tab name
  @Output() onTabChange = new EventEmitter<string>();

  // Tab clicked event (tab selected)
  selectTab(tab: string) {
    this.activeTab = tab;
    this.onTabChange.emit(tab);
  }
}
