import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyTicketsComponent } from './my-tickets.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('MyTicketsComponent', () => {
  let component: MyTicketsComponent;
  let fixture: ComponentFixture<MyTicketsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyTicketsComponent, RouterTestingModule, HttpClientTestingModule]
    }).compileComponents();
    fixture = TestBed.createComponent(MyTicketsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());
  it('selected starts undefined', () => expect(component.selected()).toBeUndefined());
  it('setStatus updates selected', () => { component.setStatus('Closed'); expect(component.selected()).toBe('Closed'); });
  it('clearStatus works', () => { component.setStatus('New'); component.setStatus(undefined); expect(component.selected()).toBeUndefined(); });
  it('countByStatus returns 0 for empty list', () => expect(component.countByStatus('New')).toBe(0));
  it('prioBg returns correct color', () => expect(component.prioBg('High')).toBe('#dc2626'));
  it('isOverdue true for past date', () => expect(component.isOverdue('2000-01-01')).toBeTrue());
});
