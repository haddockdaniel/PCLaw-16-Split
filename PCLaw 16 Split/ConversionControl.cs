using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_16_Split
{
    public class ConversionControl
    {
        private bool AR = false; //used to store whether we process AR because we need to know when we run trust
        private bool WipFees = false;
        private bool WipDisb = false;
        private bool Trust = false;

        public string SplitLawyers(string lawyers, bool run, string connString)
        {
            string error = "Lawyers: ";
            string sQuery = "UPDATE LawInf SET LawInfStatus = 1, LawInfFlags = 0 WHERE LawyerID NOT IN (" + lawyers + ")" ;
            error = SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }
        
        public string SplitMatters(string lawyers, bool run, string connString)
        { //add here some capability for log information.
            string sQuery = "";
            string error = "Matters: ";
            //TABLES HANDLED HERE
            //MatInf, MatBal, MatAudt, MattBill, Party,  CaseLwyr

            //sQuery = "DELETE FROM MattInf WHERE MatterInfoRespLwyr not in " + SplitSelections.KeepLawyerIDs;
            //SplitFunctions.RunSQLCommand(sQuery, connString);

          //  if (SplitSelections.ArchivedMatters == false)
          //  {
           //     //sQuery = "DELETE FROM MattInf WHERE MatterInfoStatus = 1";
          //      sQuery = "UPDATE MattInf SET MatterInfoStatus = 2, MatterInfoMatterNum = 'P' + substring(MatterInfoMatterNum, 1, 25)  WHERE MatterInfoStatus = 1";
          //      SplitFunctions.RunSQLCommand(sQuery, connString);
          //  }
            //where resp lawyer is not in the list
            sQuery = @"UPDATE MattInf set MatterInfoStatus = 2, MatterInfoMatterNum = 'P*' + MatterInfoMatterNum  
WHERE MatterInfoRespLwyr not in (" + lawyers + ") AND MatterInfoStatus = 0";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"UPDATE MattInf SET MatterInfoStatus = 2, MatterInfoMatterNum = 'P' + substring(MatterInfoMatterNum, 1, 25)  
WHERE  MatterInfoRespLwyr not in (" + lawyers + ") AND MatterInfoStatus = 1";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //add to remove matbal, matbill, party, etc.
            sQuery = "UPDATE MattBal SET MatterInfoStatus = 2 WHERE MatterID in (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) AND MatterInfoStatus  != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "UPDATE MattAudt SET MatterAuditStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "UPDATE MattBill SET MatterBillSettingsStatus = 2 WHERE MatterBillSettingsSeqID in (select MatterInfoSpareLong2 from MattInf where MatterInfoStatus = 2) AND MatterBillSettingsStatus != 2 ";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "UPDATE Party SET PartyStatus = 2 WHERE MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) AND PartyStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "UPDATE CaseLwyr SET CaseLawyerStatus = 2 WHERE MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) AND CaseLawyerStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;

            //deletions can be done later.
            //sQuery = "DELETE FROM MattInf WHERE MatterInfoStatus = 2";
            //SplitFunctions.RunSQLCommand(sQuery, connString);

        }

        public string SplitClients(string lawyers, bool run, string connString)
        {
            string error = "Clients: ";
            string sQuery = @"
UPDATE ClntInf SET ClientInfoStatus = 2
WHERE ClientInfoClientID NOT IN  (SELECT DISTINCT ClientInfoClientID FROM MattInf WHERE MatterInfoStatus != 2) 
AND ClientInfoStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
            UPDATE Contact 
SET ContactStatus = 2
WHERE PersonID IN (SELECT PersonInfoID FROM Person WHERE PersonInfoStatus = 2) 
AND ContactStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }

        public string SplitDiary(string lawyers, bool run, string connString)
        {
            string error = "Diary: ";
            string sQuery = "";
            //tables - diary and the link tables. we remove entries for inactive timemeepers if they aren not linked to existing matters.
            //if no active timekeeper left linked to the entry then make the resp lawyer associated with the entry.
            //Entries for removed matters
            sQuery = @"
UPDATE Diary 
SET DiaryEntryStatus = 2 
WHERE MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) 
AND DiaryEntryStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //Entries with no matters, and no active lawyer is associated with them
            sQuery = @"
UPDATE Diary
SET DiaryEntryStatus = 2 
FROM
(
	SELECT * FROM 
	(SELECT DiaryEntryID EntID FROM Diary WHERE MatterID = 0 ) d
	LEFT OUTER JOIN
	(SELECT ItemID, COUNT(*) ActvLwrs FROM ILLink WHERE LawyerID  IN (";
            sQuery += lawyers;
            sQuery += @") GROUP BY ItemID) tk
	ON d.EntID = tk.ItemID
	WHERE ItemID IS NULL
) a
WHERE DiaryEntryID = EntID AND ISNULL(ActvLwrs, 0) = 0 AND DiaryEntryStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE ILLink 
SET ILLinkStatus = 2 
WHERE ItemID IN (SELECT DiaryEntryID FROM Diary WHERE DiaryEntryStatus = 2) 
AND ILLinkStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE CILink 
SET CILinkStatus = 2 
WHERE ItemID IN (SELECT DiaryEntryID FROM Diary WHERE DiaryEntryStatus = 2) 
AND CILinkStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //Here will be left entries associated with matters - and there may be a situation that the entry has no active timekeeper.
            //We need to make on record associated with the matter responsible lawyer.
            sQuery = @"
UPDATE
ILLink
SET LawyerID = MatterInfoRespLwyr 
FROM
(
	SELECT DiaryEntryID, MatterInfoRespLwyr FROM
	(
		SELECT DiaryEntryID, MatterInfoRespLwyr FROM Diary DD
		LEFT OUTER JOIN MattInf MM ON DD.MatterID = MM.MatterID
		WHERE DD.MatterID != 0 AND DiaryEntryStatus != 2 
	) d
	LEFT OUTER JOIN 
	(
		SELECT ItemID, COUNT(*) CntLawyer FROM ILLink 
		WHERE LawyerID IN (SELECT LawyerID FROM LawInf WHERE LawInfStatus = 0) and ILLinkStatus != 2
		GROUP BY ItemID
	) il ON DiaryEntryID = ItemID  
	WHERE ISNULL(CntLawyer, 0) = 0
) IL WHERE ItemID = DiaryEntryID ";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }

        public string SplitVendors(string lawyers, bool run, string connString)
        {
            string error = "Vendors: ";
            string sQuery = "";

            sQuery = @"UPDATE Contact SET ContactStatus = 2 
WHERE PersonID in (SELECT APVendorListPersonID FROM APVendLi) AND ContactStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "UPDATE APVendLi SET APVendorListStatus = 2 WHERE APVendorListStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            error = error + "\r\n" + CleanUpTransactions(connString);
            return error;
        }

        public string CleanUpTransactions(string connString)
        {
            //why no gbcomm?
            string error = "";
            string sQuery = "";
            sQuery = @"
UPDATE TranIDX 
SET TranIndexStatus = 2
WHERE MatterID > 0 
AND MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) 
AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX 
SET TranIndexStatus = 2
WHERE TranIndexEntryType BETWEEN 1900 AND 1905
AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE GBAlloc 
SET GBankAllocInfStatus = 2 
WHERE MatterID > 0 AND 
MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) 
AND GBankAllocInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE GBAlloc 
SET GBankAllocInfStatus = 2 
WHERE GBankAllocInfEntryType BETWEEN 1900 AND 1905 
AND GBankAllocInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;
        }

        public string SplitAR(string lawyers, bool run, string connString)
        {
            string error = "AR: ";
            AR = run;
            string sQuery = @"
UPDATE ARInv 
SET ARInvoiceStatus = 2  ";

            if (run)
            {
                sQuery += @" 
WHERE MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2)
AND ARInvoiceStatus != 2";
            }
            else
            {
                sQuery += @" 
WHERE ARInvoiceStatus != 2";
            }

            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE TranIndexSequenceID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) 
AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //TIME
            sQuery = @"
UPDATE TimeEnt SET TimeEntryStatus = 2 
WHERE TimeEntryInvID != 0 AND TimeEntryInvID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) AND TimeEntryStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE  TranIndexSequenceID IN (SELECT EntryID FROM TimeEnt WHERE TimeEntryStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //EXP
            sQuery = @"
UPDATE GBAlloc SET GBankAllocInfStatus = 2 
WHERE GBankAllocInfInvID != 0 
AND  GBankAllocInfInvID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) 
AND GBankAllocInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE  TranIndexSequenceID IN (SELECT GBankAllocInfAllocID FROM GBAlloc WHERE GBankAllocInfStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //AR LAWYER SPLITS
            sQuery = @"
UPDATE ARLwySpl SET ARLawyerSplitStatus = 2 
WHERE InvoiceID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) AND ARLawyerSplitStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //WRITE OFFS
            sQuery = @"
UPDATE ARWO SET ARWriteOffStatus = 2 
WHERE ARWriteOffInvID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) AND ARWriteOffStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE TranIndexSequenceID IN (SELECT WOID FROM ARWO WHERE ARWriteOffStatus = 2)  AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //RECEIPTS ALLOCATIONS
            sQuery = @"
UPDATE GBRcptA SET GBankARRcptAllocStatus = 2 
WHERE GBankARRcptAllocEntryType != 64 AND GBankARRcptAllocInvID IN (SELECT InvoiceID FROM ARInv WHERE ARInvoiceStatus = 2) AND GBankARRcptAllocStatus !=2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            //MatBal table
            sQuery = @"
update MattBal
 set [MatterInfoARBal] = 0 ,[MatterInfoTotFees] = 0 ,[MatterInfoBilledFees] = 0  ,[MatterInfoTotDisbs] = 0
 ,[MatterInfoBilledDisbs] = 0 ,[MatterInfoTotRcpts] = 0 ,[MatterInfoBilledRcpts] = 0 ,[MatterInfoLastRcptAmount] = 0
 ,[MatterInfoRtnrBal] = 0 ,[MatterInfoTrustBal1] = 0 ,[MatterInfoTrustBal2] = 0 ,[MatterInfoTrustBal3] = 0 ,[MatterInfoTrustBal4] = 0
 ,[MatterInfoTrustBal5] = 0 ,[MatterInfoTrustBal6] = 0 ,[MatterInfoTrustBal7] = 0 ,[MatterInfoTrustBal8] = 0 ,[MatterInfoTrustBal9] = 0 
 ,[MatterInfoTrustBal10] = 0 ,[MatterInfoTotalSeconds] = 0 ,[MatterInfoBilledSeconds] = 0 ,[MatterInfoTaxFees] = 0
 ,[MatterInfoTaxDisbs] = 0 ,[MatterInfoTrustBal] = 0 ,[MatterInfoAdvanceBal] = 0 where MatterInfoStatus = 0";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;
        }

        public string SplitTrust(string lawyers, bool run, string connString)
        {
            Trust = run;
            string error = "Trust: ";
            string sQuery = @"UPDATE TBAlloc SET TBankAllocInfoStatus = 2 ";

            if (run)
            {
                sQuery += @"WHERE MatterID IN (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2) AND TBankAllocInfoStatus != 2";
            }
            else
            {
                sQuery += @"WHERE TBankAllocInfoStatus != 2";
            }
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            if (AR == false)
            {
                sQuery = @"
UPDATE TBAlloc SET TBankAllocInfInvID = 0, TBankAllocInfInvDate = 0, TBankAllocInfInvNumber = 0 
WHERE TBankAllocInfoStatus != 2 AND TBankAllocInfInvID != 0";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            }

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 WHERE TranIndexSequenceID IN (SELECT TBankAllocInfAllocID FROM TBAlloc 
WHERE TBankAllocInfoStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TBComm 
SET TBankCommInfStatus = 2
WHERE TBankCommInfSequenceID NOT IN 
(
	SELECT DISTINCT TBankAllocInfoCheckID
	FROM  TBAlloc WHERE TBankAllocInfoStatus != 2 
) 
AND TBankCommInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"UPDATE TranIDX SET TranIndexStatus = 2 
WHERE TranIndexSequenceID IN (SELECT TBankCommInfSequenceID FROM TBComm WHERE TBankCommInfStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            error = error + "\r\n" + RemoveEntriesWithNoMatters(connString);
            error = error + "\r\n" + RemovejournalEntries(connString);
            error = error + "\r\n" + CleanUpTaxes(connString);
            error = error + "\r\n" + RemoveDeletedEntries(connString);
            error = error + "\r\n" + MakeAPEntryIntoCheck(connString);
            error = error + "\r\n" + RemoveLawyersWithNoTrans(connString);
            error = error + "\r\n" + CleanAllFinancesIfNoneBroughtOver(connString);

            return error;
        }

        public string SplitWIPExpenses(string lawyers, bool run, string connString)
        {
            WipDisb = run;
            string error = "WIP Disb: ";
            string sQuery = @"
UPDATE GBAlloc SET GBankAllocInfStatus = 2 
WHERE GBankAllocInfInvID = 0 
AND GBankAllocInfEntryType IN (1400, 1600, 1650,6500)
AND GBankAllocInfStatus != 2 ";

            if (run)
            {
                sQuery += @"
AND MatterID in (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2)";
            }

            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE  TranIndexSequenceID IN (SELECT GBankAllocInfAllocID FROM GBAlloc WHERE GBankAllocInfStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;

        }

        public string SplitWIPFees(string lawyers, bool run, string connString)
        {
            WipFees = run;
            string error = "Wip Fees: ";
            string sQuery = @"
select * from TimeEnt 
WHERE TimeEntryInvID = 0 
AND TimeEntryStatus != 2 ";


            if (run)
            {
                sQuery += @" 
AND MatterID in (SELECT MatterID FROM MattInf WHERE MatterInfoStatus = 2)";
            }

            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE  TranIndexSequenceID IN (SELECT EntryID FROM TimeEnt WHERE TimeEntryStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;
        }

        public string CleanAllFinancesIfNoneBroughtOver(string connString)
        {
            string error = "";
            string sQuery = "";
            if (!AR && !Trust && !WipFees && !WipDisb) //only if they choose not to incliude any financials
            {
                //clears all finances and balances as well as closed months and GL balances
                sQuery = @"delete FROM TBBals";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from ARInv";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from ARLwySpl";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete FROM ARWO";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from TranIDX";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from APInv";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from BankInt";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GBComm";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GBAlloc";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @" delete from GBBals";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GBMemTrn";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GBRcptA";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GBRecInf";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
                sQuery = @"delete from GLActBal";
                error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            }
            return error;

        }
       
        public string RemoveEntriesWithNoMatters(string connString)
        {//check gballoc and remove gballoc gbcoms and ap if they have no allocation with matters
            string error = "";
            string sQuery = @"
UPDATE GBAlloc  SET GBankAllocInfStatus = 2
WHERE MatterID = 0 AND GBankAllocInfStatus != 2
AND GBankAllocInfCheckID NOT IN 
	(
		SELECT DISTINCT GBankAllocInfCheckID
		FROM GBAlloc 
		WHERE MatterID != 0 AND GBankAllocInfStatus != 2 
	)";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);


            sQuery = @"
UPDATE GBAlloc  SET GBankAllocInfAmount = 0.0
WHERE MatterID = 0 AND GBankAllocInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            //remove deleted entries. clean up tran index
            //remove the reversing entry number in tran
            //if any tran type is not checked - remove it here
            //remove gst opening balances from tables. type 1900 - 1905

            sQuery = @"
UPDATE GBComm 
SET GBankCommInfStatus = 2
WHERE GBankCommInfID NOT IN (SELECT DISTINCT GBankAllocInfCheckID FROM GBAlloc WHERE GBankAllocInfStatus != 2)
AND GBankCommInfStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE APInv 
SET APInvoiceStatus = 2
WHERE APInvoiceID NOT IN (SELECT DISTINCT GBankAllocInfCheckID FROM GBAlloc WHERE GBankAllocInfStatus != 2) 
AND APInvoiceStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
UPDATE TranIDX SET TranIndexStatus = 2 
WHERE  TranIndexSequenceID IN (SELECT APInvoiceID FROM APInv WHERE APInvoiceStatus = 2) AND TranIndexStatus != 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            //            sQuery = @"
            //UPDATE APInv 
            //SET APInvoiceStatus = 2
            //WHERE APInvoiceID NOT IN (SELECT DISTINCT GBankAllocInfCheckID FROM GBAlloc WHERE GBankAllocInfStatus != 2) 
            //AND APInvoiceStatus != 2";
            //SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }

        public string RemovejournalEntries(string connString)
        {
            string error = "";
            string sQuery = "";
            sQuery = "UPDATE TranIDX SET TranIndexStatus = 2 WHERE TranIndexEntryType = 4097";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "UPDATE GJEntry SET GJEntryStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "UPDATE GJAlloc SET GJAllocationStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }

        public string CleanUpTaxes(string connString)
        {
            string error = "";
            string sQuery = "";
            //101 = e EXEMPT
            //110 = n No
            //121 = y Yes
            //122 = z Zero - remittance of GST, HST.
            sQuery = @"UPDATE GBComm SET GBankCommInfGSTCat = 101 , GBankCommInfGSTAmount = 0.0  
WHERE GBankCommInfEntryType IN (1300, 1400) AND  GBankCommInfGSTCat IN(110, 121)";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;
        }

        public string RemoveDeletedEntries(string connString)
        {
            string error = "";
            string sQuery = "DELETE FROM TranIDX WHERE TranIndexStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "UPDATE TranIDX SET TranIndexRevEntry = 0 WHERE  TranIndexRevEntry != 0";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM GBComm WHERE GBankCommInfStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM GBAlloc WHERE GBankAllocInfStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM GBRcptA WHERE GBankARRcptAllocStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM GBRecInf";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM APInv WHERE APInvoiceStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM Diary WHERE DiaryEntryStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM ILLink WHERE ILLinkStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE  FROM CILink WHERE CILinkStatus = 2";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "DELETE FROM GBMemTrn";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            sQuery = "DELETE FROM TBMemTrn";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;

        }

        public string MakeAPEntryIntoCheck(string connString)
        {
            string error = "";
            string sQuery = @"UPDATE TranIDX 
SET TranIndexEntryType = 1400
WHERE TranIndexSequenceID IN 
(
	SELECT GBankAllocInfAllocID FROM GBAlloc  WHERE GBankAllocInfEntryType = 6500
	UNION 
	SELECT GBankAllocInfCheckID FROM GBAlloc   WHERE GBankAllocInfEntryType = 6500
); ";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"UPDATE GBAlloc SET GBankAllocInfEntryType = 1400 WHERE GBankAllocInfEntryType = 6500;";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"
INSERT INTO [GBComm]
([GBankCommInfStatus],      [GBankCommInfID],           [GBankCommInfDate],   [GBankCommInfRemitDate],  [GBankCommInfAccountID],
[GBankCommInfBankRecID],    [GBankCommInfEntryType],    [GBankCommInfGSTCat], [GBankCommInfARAllocFlg], [EOMID],
[GBankCommInfDepSlipID],    [GBankCommInfCashOrCheck],  [GBankCommInfAmount], [GBankCommInfGSTAmount],  [GBankCommInfCheck],
[GBankCommInfInvoice],      [GBankCommInfClientCheck],  [GBankCommInfPaidTo], [GBankCommInfOrigBank],   [GBankCommInfDrawer])
select 
0, APInvoiceID, [APInvoiceEntryDate], 0, 1, 
0, 1400, 102, 0, 0, 
0, 1, [APInvoiceTotBasePST], 0, '',
'', '', 'A/P purchase: ' + APVendorListSortName, '', ''
--select *
from APInv i
left outer join APVendLi v on i.APInvoiceVendorID = v.APVendorListID;";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "DELETE FROM APInv;";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = "DELETE FROM GBRcptA WHERE GBankARRcptAllocEntryType = 64";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            return error;
        }

        public string RemoveLawyersWithNoTrans(string connString)
        {
            string error = "";
            string sQuery = "";
            sQuery = @"UPDATE GLAcct 
SET GLAccountStatus = 2, GLAccountNickName = GLAccountNickName + '~*~'
WHERE GLAccountStatus != 2 
AND GLAccountForLawyer IN
(
	SELECT LawyerID FROM LawInf 
	WHERE [LawInfStatus] = 1 
	AND LawyerID NOT IN
	(
		SELECT MatterInfoRespLwyr L FROM MattInf WHERE MatterInfoStatus != 2
		UNION
		SELECT MatterInfoRefLwyr L FROM MattInf WHERE MatterInfoStatus != 2
		UNION
		SELECT LawyerID L FROM TimeEnt WHERE TimeEntryStatus != 2
		UNION 
		SELECT ARLawyerSplitLawyerID L FROM ARLwySpl WHERE ARLawyerSplitStatus != 2
	)
)";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);

            sQuery = @"UPDATE LawInf 
SET [LawInfStatus] = 2, LawInfNickName = LawInfNickName +  '~*~'
WHERE [LawInfStatus] = 1 
AND LawyerID NOT IN
(
	SELECT MatterInfoRespLwyr L FROM MattInf WHERE MatterInfoStatus != 2
	UNION
	SELECT MatterInfoRefLwyr L FROM MattInf WHERE MatterInfoStatus != 2
	UNION
	SELECT LawyerID L FROM TimeEnt WHERE TimeEntryStatus != 2
	UNION 
	SELECT ARLawyerSplitLawyerID FROM ARLwySpl WHERE ARLawyerSplitStatus != 2
)";
            error = error + "\r\n" + SplitFunctions.RunSQLCommand(sQuery, connString);
            return error;
        }

    }
}
