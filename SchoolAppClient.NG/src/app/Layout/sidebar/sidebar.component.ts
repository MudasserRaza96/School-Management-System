import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { CommonServices } from '../../Services/common.service';
import { filter } from 'rxjs/operators';

export interface MenuItem {
  label: string;
  icon: string;
  route?: string;
  children?: MenuItem[];
}

export interface MenuSection {
  title: string;
  items: MenuItem[];
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  isSidebarVisible = true;
  isCollapsed = false;
  openSubmenus: { [key: string]: boolean } = {};

  menuSections: MenuSection[] = [
    {
      title: 'MAIN MENU',
      items: [
        { label: 'Home', icon: 'home', route: '/home' },
        { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
        { label: 'Grade', icon: 'grade', route: '/grades' },
        { label: 'Schedule', icon: 'schedule', route: '/schedule' },
        { label: 'Time Table', icon: 'access_time', route: '/timetable' },
        { label: 'Student', icon: 'school', route: '/student' },
        {
          label: 'Employee',
          icon: 'badge',
          children: [
            { label: 'Departments', icon: 'business', route: '/departments' },
            { label: 'Staff', icon: 'group', route: '/staff-list' },
            { label: 'Staff Salaries', icon: 'payments', route: '/staff-salaries' }
          ]
        },
        {
          label: 'Activities',
          icon: 'assignment',
          children: [
            { label: 'Attendances', icon: 'event_available', route: '/attendanceList' },
            { label: 'Marks Entry', icon: 'edit_note', route: '/marksentrynewList' }
          ]
        },
        {
          label: 'Examination',
          icon: 'quiz',
          children: [
            { label: 'Exam Types', icon: 'category', route: '/exam-types' },
            { label: 'Exam Schedule', icon: 'event_note', route: '/examSchedule' },
            { label: 'Exam Schedule Std', icon: 'date_range', route: '/examScheduleStandard' }
          ]
        },
        {
          label: 'Subjects',
          icon: 'menu_book',
          children: [
            { label: 'Subject List', icon: 'list_alt', route: '/subjects' },
            { label: 'Standards', icon: 'layers', route: '/standards' }
          ]
        },
        {
          label: 'Payment',
          icon: 'account_balance_wallet',
          children: [
            { label: 'Fee Types', icon: 'label', route: '/fee-types' },
            { label: 'Fee Amount', icon: 'attach_money', route: '/fees' },
            { label: 'Payment Details', icon: 'receipt_long', route: '/pmaymentdetails' },
            { label: 'Monthly Payment', icon: 'calendar_month', route: '/monthlypayment' },
            { label: 'Other Payment', icon: 'credit_card', route: '/otherpayment' }
          ]
        }
      ]
    },
    {
      title: 'SYSTEM',
      items: [
        { label: 'Settings', icon: 'settings', route: '/settings' }
      ]
    }
  ];

  constructor(
    private sidebarService: CommonServices,
    private router: Router
  ) { }

  ngOnInit() {
    this.sidebarService.sidebarVisibility$.subscribe((isVisible) => {
      this.isSidebarVisible = isVisible;
    });

    this.sidebarService.sidebarCollapsed$.subscribe((isCollapsed) => {
      this.isCollapsed = isCollapsed;
    });

    // Automatically expand parent menu of active route
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.expandActiveMenu(event.urlAfterRedirects || event.url);
      });

    this.expandActiveMenu(this.router.url);
  }

  toggleSubmenu(label: string) {
    if (this.isCollapsed) {
      // Un-collapse sidebar when expanding submenus in collapsed mode
      this.sidebarService.setCollapsed(false);
    }
    this.openSubmenus[label] = !this.openSubmenus[label];
  }

  isSubmenuOpen(label: string): boolean {
    return !!this.openSubmenus[label];
  }

  private expandActiveMenu(currentUrl: string) {
    for (const section of this.menuSections) {
      for (const item of section.items) {
        if (item.children) {
          const hasActiveChild = item.children.some(child => child.route && currentUrl.includes(child.route));
          if (hasActiveChild) {
            this.openSubmenus[item.label] = true;
          }
        }
      }
    }
  }

  isChildActive(item: MenuItem): boolean {
    if (!item.children) return false;
    const currentUrl = this.router.url;
    return item.children.some(child => child.route && currentUrl.includes(child.route));
  }
}

