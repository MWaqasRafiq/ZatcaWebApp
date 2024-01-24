-- =============================================  
-- Author:  <Author,,Waqas>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
CREATE OR ALTER procedure  [dbo].[SP_RPT_INVOICE]  
	@IRN nvarchar(max) = null,  
	@DocType nvarchar(max)= 'ALL',  
	@ActionStatus nvarchar(max)= 'ALL',  
	@TransactionDate nvarchar(max)= null,  
	@StoreNo nvarchar(max)= 'ALL',  
	@TerminalNo nvarchar(max)= null,  
	@TrxNo nvarchar(max)= null,  
	@Source nvarchar(max)= null,  
	@FullResponse nvarchar(max)= null,  
	@CustIdentifier nvarchar(max)  = null,
	@PageNumber INT = 1, 
	@PageSize INT = 10
	--,@TotalRecords INT = null OUT
AS  
BEGIN  
 SET NOCOUNT ON;  
  
  --getting storeno, termno, trxno from IRN  
  if(@IRN !='' and @DocType='B2C')  
  begin  
  
  if(@CustIdentifier='1')  --F  
  begin  
        set @StoreNo=(select SUBSTRING(@IRN,0,6));  
  --set @TransactionDate=(select SUBSTRING(@IRN,6,10));  
  set @TerminalNo=(select SUBSTRING(@IRN,16,3));  
  set @TrxNo=(select SUBSTRING(@IRN,19,4));    
  end  
  else  
  begin  
         set @StoreNo=(select SUBSTRING(@IRN,0,6));  
  set @TerminalNo=(select SUBSTRING(@IRN,6,3));  
  set @TrxNo=(select SUBSTRING(@IRN,9,4));    
  --set @TransactionDate=(select SUBSTRING(@IRN,13,10));  
  end  
  end  
  
  if(isnull(@FullResponse,'') ='') set @FullResponse=(select min(TransactionDate) from Invoices (nolock));  
  if(isnull(@TransactionDate,'') ='') set @TransactionDate=(select max(TransactionDate) from Invoices (nolock));  
  
	WITH TempResult AS
	(
		SELECT top 100 
		  --Id,
		   [IRN]  
		  ,[UUID]  
		  ,[XMLFileName]  
		  --,TaxTotal  
		  ,[DocType]  
		  ,[Action]  
		  ,[ActionStatus]  
		  ,[TransactionDate]  
		  ,[StoreNo]  
		  ,[TerminalNo]  
		  ,[TrxNo]  
		  ,case when  LTRIM(RTRIM(DocType))='B2B' then ''  
			when ActionStatus='Reported' THEN ''  
			else  CAST(cast(TimeleftToReport as time(0)) AS VARCHAR) end as TimeleftToReport  
		  ,[ReportedIn]  
		  ,[Source]  
		  ,[ActionCount]  
		  ,[ErrorCount]  
		  ,[WarningCount]  
		  ,[CreatedDate]  
		  ,[UpdatedDate]  
	  FROM [dbo].[Invoices] (nolock)  
	  where   TransactionDate >=cast(@FullResponse as datetime) 
		and   TransactionDate <=cast(@TransactionDate as datetime) 
		and (TerminalNo=@TerminalNo or isnull(@TerminalNo,'') ='' )  
		and (TrxNo=@TrxNo or isnull(@TrxNo,'') ='' )  
		and (ActionStatus=@ActionStatus or isnull(@ActionStatus,'') ='ALL' )  
		and (DocType=@DocType or @DocType ='ALL')  
		and (StoreNo=@StoreNo or @StoreNo ='ALL')  
		and (IRN=@IRN or isnull(@IRN,'') ='' )  
	), 
	TempCount AS
	(
		SELECT COUNT(*) AS  TotalRows FROM TempResult
	)
	SELECT *
	FROM TempResult, TempCount
	ORDER BY TempResult.TransactionDate
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY; 


  --SELECT top 100 Id,  
  --     [IRN]  
  --    ,   [UUID]  
  --    ,[XMLFileName]  
  -- ,TaxTotal  
  --    ,[DocType]  
  --    ,[Action]  
  --    ,[ActionStatus]  
  --    ,[TransactionDate]  
  --    ,[StoreNo]  
  --    ,[TerminalNo]  
  --    ,[TrxNo]  
  --    ,case when  LTRIM(RTRIM(DocType))='B2B' then ''  
  --       when ActionStatus='Reported' THEN ''  
  -- else  CAST(cast(TimeleftToReport as time(0)) AS VARCHAR) end as TimeleftToReport  
  --    ,[ReportedIn]  
  --    ,[Source]  
  --    ,[ActionCount]  
  --    ,[ErrorCount]  
  --    ,[WarningCount]  
  --    ,[CreatedDate]  
  --    ,[UpdatedDate]  
  --FROM [dbo].[Invoices] (nolock)  
  --where   TransactionDate >=cast(@FullResponse as datetime) and   TransactionDate <=cast(@TransactionDate as datetime) and  
  --       (TerminalNo=@TerminalNo or isnull(@TerminalNo,'') ='' )  
  --  and (TrxNo=@TrxNo or isnull(@TrxNo,'') ='' )  
  --  and (ActionStatus=@ActionStatus or isnull(@ActionStatus,'') ='ALL' )  
  --  and (DocType=@DocType or @DocType ='ALL')  
  --   and (StoreNo=@StoreNo or @StoreNo ='ALL')  
  --     and (IRN=@IRN or isnull(@IRN,'') ='' )  
  --  order by TransactionDate desc  
 
  
 --  Select  count(*) as Total,  
 --  SUM(CASE WHEN TB.ActionStatus='Cleared' THEN 1 ELSE 0 END) as Cleared ,  
 --  SUM(CASE WHEN TB.ActionStatus='Reported' THEN 1 ELSE 0 END) as Reported,  
 --  SUM(CASE WHEN TB.ActionStatus='Cleared' and WarningCount>0 THEN 1 ELSE 0 END) as ClearedWithWarning ,  
 --  SUM(CASE WHEN TB.ActionStatus='Reported' and WarningCount>0 THEN 1 ELSE 0 END) as ReportedWithWarning ,  
 --  SUM(CASE WHEN TB.ActionStatus!='Cleared' and DocType='B2B' THEN 1 ELSE 0 END) as ClearanceFailed,  
 --  SUM(CASE WHEN TB.ActionStatus!='Reported' and DocType='B2C' THEN 1 ELSE 0 END) as ReportingFailed,  
 --  SUM(CASE WHEN TB.ActionStatus not in ('Reported','Cleared') THEN 1 ELSE 0 END) as TotalFailed  
      
 --from   
 --(select *  
 --FROM [dbo].[Invoices]   (nolock)
 -- where   TransactionDate >=cast(@FullResponse as datetime) and  TransactionDate <=cast(@TransactionDate as datetime) and  
 --        (TerminalNo=@TerminalNo or isnull(@TerminalNo,'') ='' )  
 --   and (TrxNo=@TrxNo or isnull(@TrxNo,'') ='' )  
 --   and (ActionStatus=@ActionStatus or isnull(@ActionStatus,'') ='ALL' )  
 --   and (DocType=@DocType or @DocType ='ALL')  
 --    and (StoreNo=@StoreNo or @StoreNo ='ALL')  
 --      and (IRN=@IRN or isnull(@IRN,'') ='' )  
 --   ) TB  
  
      
END  