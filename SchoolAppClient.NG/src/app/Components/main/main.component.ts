import { Component, OnInit } from '@angular/core';
import { CommonServices } from '../../Services/common.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent implements OnInit {
  isSidebarVisible = true;
  isCollapsed = false;

  constructor(private sidebarService: CommonServices) { }

  ngOnInit() {
    this.sidebarService.sidebarVisibility$.subscribe((isVisible) => {
      this.isSidebarVisible = isVisible;
    });

    this.sidebarService.sidebarCollapsed$.subscribe((isCollapsed) => {
      this.isCollapsed = isCollapsed;
    });
  }

  closeMobileSidebar() {
    if (window.innerWidth <= 768 && this.isSidebarVisible) {
      this.sidebarService.toggleSidebar();
    }
  }
}

