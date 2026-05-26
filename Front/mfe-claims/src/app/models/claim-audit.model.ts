export interface ClaimAudit {
    id: number;
    claimId: number;
    changedBy: string;
    fieldChanged: string;
    oldValue: string;
    newValue: string;
    changedAt: string;
}