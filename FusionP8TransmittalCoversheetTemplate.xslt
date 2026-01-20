<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sys="urn:system">
    <xsl:key name="recipient" match="action/argument[@name='Recipient']/value/text()" use="."/>
    <xsl:template match="document | logical-document" >
        <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">
			<fo:layout-master-set>                      
				<fo:simple-page-master  master-name="simple" margin-top="0.5in" margin-bottom="0.5in" margin-left="0.5in" margin-right="0.5in">                  
					<fo:region-body/>
				</fo:simple-page-master>
			</fo:layout-master-set> 
			<fo:page-sequence master-reference="simple">    
				<fo:flow flow-name="xsl-region-body" font-family="Tahoma" font-size="10pt">   
					<fo:block>
					   <fo:inline>Document:  </fo:inline>
					   
						<fo:inline>Class = <xsl:value-of  select="argument[@name='Class']"/></fo:inline>
												
						<fo:inline>, Doc ID = <xsl:value-of  select="argument[@name='PSEGDocumentID']"/></fo:inline>
												
						<fo:inline>, Sheet = <xsl:value-of  select="argument[@name='Sheet']"/></fo:inline>
												
						<fo:inline>, Rev = <xsl:value-of  select="argument[@name='Revision']"/> </fo:inline>
												
					</fo:block>
					<fo:block>	
						<fo:inline>Original Drafter : <xsl:value-of  select="sys:GetPrincipalArgument(argument[@name='OriginalDrafter'], 'DisplayName')"/></fo:inline>
					</fo:block>		
					<fo:block>	
						<fo:inline>Document Issuer : <xsl:value-of  select="sys:GetPrincipalArgument(argument[@name='Drafter'], 'DisplayName')"/></fo:inline>
					</fo:block>					
					<fo:block>	
					
						<fo:inline>Reviewer : <xsl:value-of  select="sys:GetPrincipalArgument(argument[@name='PeerReviewer'], 'DisplayName')"/></fo:inline>
					</fo:block>							
					<fo:block>	
						
						<fo:inline>Approver : <xsl:value-of  select="sys:GetPrincipalArgument(argument[@name='Approver'], 'DisplayName')"/></fo:inline>
					</fo:block>							
					<fo:block>
						
						<fo:inline>   Approval Date : <xsl:value-of  select="argument[@name='ApprovalDate']"/></fo:inline>
					</fo:block>
				</fo:flow>
			</fo:page-sequence>
		</fo:root>
    </xsl:template>
</xsl:stylesheet>
