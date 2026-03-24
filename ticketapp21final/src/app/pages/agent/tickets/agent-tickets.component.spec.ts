import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AgentTicketsComponent } from './agent-tickets.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AgentTicketsComponent', () => {
  let f: ComponentFixture<AgentTicketsComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AgentTicketsComponent,RouterTestingModule,HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(AgentTicketsComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('prioBg correct', () => expect(f.componentInstance.prioBg('Low')).toBe('#059669'));
});
