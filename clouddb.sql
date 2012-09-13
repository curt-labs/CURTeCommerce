SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Country](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[abbr] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[longAbbr] [varchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK__Country__3214EC2723202489] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[States]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[States](
	[stateID] [int] IDENTITY(1,1) NOT NULL,
	[state] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[abbr] [varchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[countryID] [int] NOT NULL,
	[taxRate] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[stateID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__States__countryI__5224328E]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK__States__countryI__5224328E] FOREIGN KEY([countryID])
REFERENCES [dbo].[Country] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__States__countryI__5224328E]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK__States__countryI__5224328E]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__States__countryI__51300E55]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[States] ADD  CONSTRAINT [DF__States__countryI__51300E55]  DEFAULT ((1)) FOR [countryID]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Address]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Address](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[street1] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[street2] [varchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[city] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[state] [int] NOT NULL,
	[postal_code] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[cust_id] [int] NOT NULL,
	[residential] [bit] NOT NULL,
	[first] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[last] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PrimaryKey_9a489509-0b2e-40d2-9f88-6171f9aec839] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Address__state__7A672E12]') AND parent_object_id = OBJECT_ID(N'[dbo].[Address]'))
ALTER TABLE [dbo].[Address]  WITH CHECK ADD FOREIGN KEY([state])
REFERENCES [dbo].[States] ([stateID])
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_d60c279a-456d-41e1-8c87-81745aaeea6c]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [ColumnDefault_d60c279a-456d-41e1-8c87-81745aaeea6c]  DEFAULT ((0)) FOR [state]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Address__cust_id__3C34F16F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF__Address__cust_id__3C34F16F]  DEFAULT ((0)) FOR [cust_id]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Address__residen__531856C7]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF__Address__residen__531856C7]  DEFAULT ((1)) FOR [residential]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Address__active__59C55456]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF__Address__active__59C55456]  DEFAULT ((1)) FOR [active]
END
GO
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Banners]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Banners](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[image] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[title] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[body] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[link] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[order] [int] NOT NULL,
	[isVisible] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_199542dd-66ab-4407-b6e0-c353dff7cf74] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_6a29d83b-d19b-4d03-848f-d21c97f91281]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Banners] ADD  CONSTRAINT [ColumnDefault_6a29d83b-d19b-4d03-848f-d21c97f91281]  DEFAULT ((0)) FOR [order]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_e928f140-4771-4b29-98cf-37b024ac7afd]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Banners] ADD  CONSTRAINT [ColumnDefault_e928f140-4771-4b29-98cf-37b024ac7afd]  DEFAULT ((0)) FOR [isVisible]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BlogCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BlogCategories](
	[blogCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[slug] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK__BlogCate__60848BAFFDC4B814] PRIMARY KEY CLUSTERED 
(
	[blogCategoryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__BlogCateg__activ__17F790F9]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BlogCategories] ADD  CONSTRAINT [DF__BlogCateg__activ__17F790F9]  DEFAULT ((1)) FOR [active]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BlogPosts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BlogPosts](
	[blogPostID] [int] IDENTITY(1,1) NOT NULL,
	[post_title] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[slug] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[post_text] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[publishedDate] [datetime] NULL,
	[createdDate] [datetime] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[profileID] [int] NOT NULL,
	[meta_title] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[meta_description] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[keywords] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK__BlogPost__AA6A5F91E8FA1FC9] PRIMARY KEY CLUSTERED 
(
	[blogPostID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BlogPosts]') AND name = N'BlogPostAuthorID')
CREATE NONCLUSTERED INDEX [BlogPostAuthorID] ON [dbo].[BlogPosts] 
(
	[profileID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__BlogPosts__creat__1AD3FDA4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BlogPosts] ADD  CONSTRAINT [DF__BlogPosts__creat__1AD3FDA4]  DEFAULT (getdate()) FOR [createdDate]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__BlogPosts__lastM__1BC821DD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BlogPosts] ADD  CONSTRAINT [DF__BlogPosts__lastM__1BC821DD]  DEFAULT (getdate()) FOR [lastModified]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__BlogPosts__activ__1CBC4616]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BlogPosts] ADD  CONSTRAINT [DF__BlogPosts__activ__1CBC4616]  DEFAULT ((1)) FOR [active]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BlogPost_BlogCategory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BlogPost_BlogCategory](
	[postCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[blogPostID] [int] NOT NULL,
	[blogCategoryID] [int] NOT NULL,
 CONSTRAINT [PK__BlogPost__D16ED0F08097166F] PRIMARY KEY CLUSTERED 
(
	[postCategoryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__BlogPost___blogC__25518C17]') AND parent_object_id = OBJECT_ID(N'[dbo].[BlogPost_BlogCategory]'))
ALTER TABLE [dbo].[BlogPost_BlogCategory]  WITH CHECK ADD  CONSTRAINT [FK__BlogPost___blogC__25518C17] FOREIGN KEY([blogCategoryID])
REFERENCES [dbo].[BlogCategories] ([blogCategoryID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__BlogPost___blogC__25518C17]') AND parent_object_id = OBJECT_ID(N'[dbo].[BlogPost_BlogCategory]'))
ALTER TABLE [dbo].[BlogPost_BlogCategory] CHECK CONSTRAINT [FK__BlogPost___blogC__25518C17]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__BlogPost___blogP__2645B050]') AND parent_object_id = OBJECT_ID(N'[dbo].[BlogPost_BlogCategory]'))
ALTER TABLE [dbo].[BlogPost_BlogCategory]  WITH CHECK ADD  CONSTRAINT [FK__BlogPost___blogP__2645B050] FOREIGN KEY([blogPostID])
REFERENCES [dbo].[BlogPosts] ([blogPostID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__BlogPost___blogP__2645B050]') AND parent_object_id = OBJECT_ID(N'[dbo].[BlogPost_BlogCategory]'))
ALTER TABLE [dbo].[BlogPost_BlogCategory] CHECK CONSTRAINT [FK__BlogPost___blogP__2645B050]
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Cart](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[cust_id] [int] NOT NULL,
	[payment_id] [int] NOT NULL,
	[shipping_price] [decimal](18, 2) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[last_updated] [timestamp] NOT NULL,
	[ship_to] [int] NOT NULL,
	[shipping_type] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[bill_to] [int] NOT NULL,
	[tax] [decimal](18, 2) NOT NULL,
	[tracking_number] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[voided] [bit] NOT NULL,
 CONSTRAINT [PrimaryKey_c2647f75-8256-45e1-8349-e2115d3bc7f4] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND name = N'IX_Cart_Customer')
CREATE NONCLUSTERED INDEX [IX_Cart_Customer] ON [dbo].[Cart] 
(
	[cust_id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND name = N'IX_Cart_CustomerPayment')
CREATE NONCLUSTERED INDEX [IX_Cart_CustomerPayment] ON [dbo].[Cart] 
(
	[cust_id] ASC,
	[payment_id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND name = N'IX_Cart_Payment')
CREATE NONCLUSTERED INDEX [IX_Cart_Payment] ON [dbo].[Cart] 
(
	[payment_id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_77a7efe7-801b-4918-847a-4fb7f81e8224]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Cart] ADD  CONSTRAINT [ColumnDefault_77a7efe7-801b-4918-847a-4fb7f81e8224]  DEFAULT (getdate()) FOR [date_created]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Cart__bill_to__55009F39]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Cart] ADD  CONSTRAINT [DF__Cart__bill_to__55009F39]  DEFAULT ((0)) FOR [bill_to]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Cart__tax__625A9A57]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Cart] ADD  CONSTRAINT [DF__Cart__tax__625A9A57]  DEFAULT ((0)) FOR [tax]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Cart__void__04E4BC85]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Cart] ADD  DEFAULT ((0)) FOR [voided]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CartItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[partID] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[price] [decimal](18, 2) NOT NULL,
	[order_id] [int] NOT NULL,
	[shortDesc] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[upc] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[weight] [decimal](18, 0) NULL,
 CONSTRAINT [PrimaryKey_eea60f48-6095-4847-a9cd-8eccfc4ec348] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Comments](
	[commentID] [int] IDENTITY(1,1) NOT NULL,
	[blogPostID] [int] NOT NULL,
	[name] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[email] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[comment_text] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[createdDate] [datetime] NOT NULL,
	[approved] [bit] NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK__Comments__CDDE91BD8BE78CAD] PRIMARY KEY CLUSTERED 
(
	[commentID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Comments__blogPo__245D67DE]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comments]'))
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK__Comments__blogPo__245D67DE] FOREIGN KEY([blogPostID])
REFERENCES [dbo].[BlogPosts] ([blogPostID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Comments__blogPo__245D67DE]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comments]'))
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK__Comments__blogPo__245D67DE]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Comments__create__2180FB33]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF__Comments__create__2180FB33]  DEFAULT (getdate()) FOR [createdDate]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Comments__approv__22751F6C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF__Comments__approv__22751F6C]  DEFAULT ((0)) FOR [approved]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Comments__active__236943A5]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF__Comments__active__236943A5]  DEFAULT ((1)) FOR [active]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactInquiries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContactInquiries](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[phone] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[message] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[contact_type] [int] NOT NULL,
	[dateAdded] [datetime] NOT NULL,
	[followedUp] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_db39dda1-4339-473d-b70d-67bc5ff9dad5] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_9089e570-075d-4876-87b9-be19da7639ec]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContactInquiries] ADD  CONSTRAINT [ColumnDefault_9089e570-075d-4876-87b9-be19da7639ec]  DEFAULT (getdate()) FOR [dateAdded]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_76d3250f-9179-4e04-a9ff-aa9337cbee06]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContactInquiries] ADD  CONSTRAINT [ColumnDefault_76d3250f-9179-4e04-a9ff-aa9337cbee06]  DEFAULT ((0)) FOR [followedUp]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContactTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[label] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[email] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PrimaryKey_45b7ac18-9d6c-4e08-bba9-bc1a51e49e49] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentNesting]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentNesting](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[pageID] [int] NOT NULL,
	[parentID] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_8cd9af2c-b78b-4166-94f2-517568e643bf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentPage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentPage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[content] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[visible] [int] NOT NULL,
	[metaTitle] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[metaDescription] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PrimaryKey_caae9c6c-46f4-417d-9ade-0d19e96c3a15] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__ContentPa__visib__71D1E811]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentPage] ADD  CONSTRAINT [DF__ContentPa__visib__71D1E811]  DEFAULT ((0)) FOR [visible]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Customer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[password] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[fname] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[lname] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[phone] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[dateAdded] [datetime] NOT NULL,
	[isSuspended] [int] NOT NULL,
	[receiveNewsletter] [int] NOT NULL,
	[receiveOffers] [int] NOT NULL,
	[billingID] [int] NOT NULL,
	[shippingID] [int] NOT NULL,
	[isValidated] [int] NOT NULL,
	[validator] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PrimaryKey_da99b855-5f96-4c45-81d5-9afadbd275db] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_91ae4ca6-4a23-42dc-8a34-4c4222183e43]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_91ae4ca6-4a23-42dc-8a34-4c4222183e43]  DEFAULT (getdate()) FOR [dateAdded]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_5cb0ba74-63b8-4845-a7c7-06e21ce2c503]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_5cb0ba74-63b8-4845-a7c7-06e21ce2c503]  DEFAULT ((0)) FOR [isSuspended]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_5438aa2e-afe7-420d-8d0f-dabee7ee0891]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_5438aa2e-afe7-420d-8d0f-dabee7ee0891]  DEFAULT ((0)) FOR [receiveNewsletter]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_e77b10e9-a9fc-4d67-b22c-19291cafa95d]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_e77b10e9-a9fc-4d67-b22c-19291cafa95d]  DEFAULT ((0)) FOR [receiveOffers]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_ae86a5f6-3652-42b7-a455-a1e1be2b3c75]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_ae86a5f6-3652-42b7-a455-a1e1be2b3c75]  DEFAULT ((0)) FOR [billingID]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_2c7afcb7-b0af-481c-8bac-1d8a89b3bd61]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_2c7afcb7-b0af-481c-8bac-1d8a89b3bd61]  DEFAULT ((0)) FOR [shippingID]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_962a004e-3069-4548-ad98-298644869e3b]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [ColumnDefault_962a004e-3069-4548-ad98-298644869e3b]  DEFAULT ((0)) FOR [isValidated]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DistributionCenters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DistributionCenters](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Phone] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Fax] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Street1] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Street2] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[City] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PostalCode] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CountryCode] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Latitude] [decimal](18, 8) NULL,
	[Longitude] [decimal](18, 8) NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_ed832354-44ff-4e8f-ae49-f458c8e4fdd0] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Distribut__State__756D6ECB]') AND parent_object_id = OBJECT_ID(N'[dbo].[DistributionCenters]'))
ALTER TABLE [dbo].[DistributionCenters]  WITH CHECK ADD  CONSTRAINT [FK__Distribut__State__756D6ECB] FOREIGN KEY([State])
REFERENCES [dbo].[States] ([stateID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Distribut__State__756D6ECB]') AND parent_object_id = OBJECT_ID(N'[dbo].[DistributionCenters]'))
ALTER TABLE [dbo].[DistributionCenters] CHECK CONSTRAINT [FK__Distribut__State__756D6ECB]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_393afb81-a66a-444a-8740-564a67e9e069]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DistributionCenters] ADD  CONSTRAINT [ColumnDefault_393afb81-a66a-444a-8740-564a67e9e069]  DEFAULT ('US') FOR [CountryCode]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_f0a35c86-333d-46bb-a7ab-2abd118e993c]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DistributionCenters] ADD  CONSTRAINT [ColumnDefault_f0a35c86-333d-46bb-a7ab-2abd118e993c]  DEFAULT ('0') FOR [Latitude]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_9e19ee96-6e71-4556-a316-7deaef4ba24c]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DistributionCenters] ADD  CONSTRAINT [ColumnDefault_9e19ee96-6e71-4556-a316-7deaef4ba24c]  DEFAULT ('0') FOR [Longitude]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Distribut__State__540C7B00]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DistributionCenters] ADD  CONSTRAINT [DF__Distribut__State__540C7B00]  DEFAULT ((1)) FOR [State]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FAQ]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FAQ](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[question] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[answer] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[order] [int] NOT NULL,
	[topic] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_52b2859d-a2e0-41a9-a7da-557f54cc7caf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_c6365b0f-46ca-492f-b3db-ccb54a008183]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FAQ] ADD  CONSTRAINT [ColumnDefault_c6365b0f-46ca-492f-b3db-ccb54a008183]  DEFAULT ((0)) FOR [order]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_02897d5e-b343-41e0-8ec5-af9734a73eb6]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FAQ] ADD  CONSTRAINT [ColumnDefault_02897d5e-b343-41e0-8ec5-af9734a73eb6]  DEFAULT ((0)) FOR [topic]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FaqTopic]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FaqTopic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[topic] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[dateAdded] [datetime] NOT NULL,
 CONSTRAINT [PrimaryKey_1720e49e-5c99-4434-8d5a-7ef68c33b41a] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_fdc959e1-6f89-471a-8736-e014019a69fb]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FaqTopic] ADD  CONSTRAINT [ColumnDefault_fdc959e1-6f89-471a-8736-e014019a69fb]  DEFAULT (getdate()) FOR [dateAdded]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoice]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Invoice](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[number] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[dateAdded] [datetime] NOT NULL,
	[orderID] [varchar](100) NOT NULL,
	[invoiceType] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[billTo] [int] NOT NULL,
	[shipTo] [int] NOT NULL,
	[salesTax] [money] NULL,
	[billToCurrency] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[curtOrder] [int] NULL,
	[remitTo] [varchar](25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subtotal] [decimal](18, 2) NULL,
	[total] [decimal](18, 2) NULL,
	[discount] [decimal](18, 2) NULL,
	[discountTotal] [decimal](18, 2) NULL,
	[termsType] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[discountPercent] [decimal](18, 0) NULL,
	[discountDueDate] [datetime] NULL,
	[discountDueDays] [int] NULL,
	[netDueDate] [datetime] NULL,
	[netDueDays] [int] NULL,
	[termsDescription] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[paid] [bit] NOT NULL,
	[printed] [bit] NOT NULL,
	[created] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Invoice__paid__625A9A57]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT ((0)) FOR [paid]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Invoice__created__634EBE90]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (getdate()) FOR [created]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceCode]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InvoiceCode](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[invoiceID] [int] NOT NULL,
	[type] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[code] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[value] [decimal](18, 2) NOT NULL,
	[description] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InvoiceItem](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[invoiceID] [int] NOT NULL,
	[partID] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[quantity] [int] NOT NULL,
	[price] [money] NOT NULL,
	[description] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Locations](
	[locationID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[phone] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[fax] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[address] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[city] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[stateID] [int] NOT NULL,
	[zip] [int] NULL,
	[isPrimary] [int] NOT NULL,
	[latitude] [decimal](18, 8) NOT NULL,
	[longitude] [decimal](18, 8) NOT NULL,
	[places_status] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[places_reference] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[places_id] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[foursquare_id] [varchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Locations2] PRIMARY KEY CLUSTERED 
(
	[locationID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Locations2_isPrimary]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Locations] ADD  CONSTRAINT [DF_Locations2_isPrimary]  DEFAULT ((0)) FOR [isPrimary]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Locations2_latitude]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Locations] ADD  CONSTRAINT [DF_Locations2_latitude]  DEFAULT ('0') FOR [latitude]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Locations2_longitude]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Locations] ADD  CONSTRAINT [DF_Locations2_longitude]  DEFAULT ('0') FOR [longitude]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LocationServices]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LocationServices](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[locationID] [int] NOT NULL,
	[serviceID] [int] NOT NULL,
 CONSTRAINT [PrimaryKey_01980a54-7143-4629-8be0-625313538bbd] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Modules]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Modules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[path] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[parentID] [int] NOT NULL,
	[alt_text] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[inMenu] [int] NOT NULL,
	[image] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Newsletter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Newsletter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Email] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[Unsubscribe] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PrimaryKey_d10ab561-b64d-4824-9646-eec8b99116ea] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_0ff28ee6-bfe2-4117-b237-e66858a9d507]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Newsletter] ADD  CONSTRAINT [ColumnDefault_0ff28ee6-bfe2-4117-b237-e66858a9d507]  DEFAULT (getdate()) FOR [DateAdded]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Payment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NOT NULL,
	[created] [datetime] NOT NULL,
	[confirmationKey] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[status] [varchar](25) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK__Payment__3214EC27D861B917] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Payment__status__1AD3FDA4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Payment] ADD  DEFAULT ('Complete') FOR [status]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PaymentType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK__PaymentT__3214EC270A73BD5C] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Profile]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Profile](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[password] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[first] [varchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[last] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[date_added] [datetime] NULL,
	[image] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[bio] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Profile_date_added]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Profile] ADD  CONSTRAINT [DF_Profile_date_added]  DEFAULT (getdate()) FOR [date_added]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProfileModules]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProfileModules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[profileID] [int] NOT NULL,
	[moduleID] [int] NOT NULL,
 CONSTRAINT [PK_ProfileModules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduledTask]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ScheduledTask](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[url] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[runtime] [datetime] NULL,
	[lastRan] [datetime] NULL,
	[interval] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Services](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[price] [decimal](18, 2) NOT NULL,
	[hourly] [varchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PrimaryKey_b8bff07c-bae7-4810-9e80-aae3277cd7e8] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ColumnDefault_22d18f92-13db-473a-b4dc-31ce68984130]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Services] ADD  CONSTRAINT [ColumnDefault_22d18f92-13db-473a-b4dc-31ce68984130]  DEFAULT ((0)) FOR [hourly]
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SettingGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SettingGroup](
	[settingGroupID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[settingGroupID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Setting]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Setting](
	[settingID] [int] IDENTITY(1,1) NOT NULL,
	[groupID] [int] NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[value] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[isImage] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[settingID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Setting__groupID__19DFD96B]') AND parent_object_id = OBJECT_ID(N'[dbo].[Setting]'))
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD FOREIGN KEY([groupID])
REFERENCES [dbo].[SettingGroup] ([settingGroupID])
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Testimonial]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Testimonial](
	[testimonialID] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[last_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[testimonial] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[location] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[dateAdded] [datetime] NOT NULL,
	[approved] [bit] NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK__Testimon__BB60213FFFC3B1D0] PRIMARY KEY CLUSTERED 
(
	[testimonialID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
-- BCPArgs:2:[dbo].[Country] in "c:\SQLAzureMW\BCPData\dbo.Country.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:70:[dbo].[States] in "c:\SQLAzureMW\BCPData\dbo.States.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:41:[dbo].[Address] in "c:\SQLAzureMW\BCPData\dbo.Address.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:5:[dbo].[Banners] in "c:\SQLAzureMW\BCPData\dbo.Banners.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:2:[dbo].[BlogCategories] in "c:\SQLAzureMW\BCPData\dbo.BlogCategories.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[BlogPosts] in "c:\SQLAzureMW\BCPData\dbo.BlogPosts.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[BlogPost_BlogCategory] in "c:\SQLAzureMW\BCPData\dbo.BlogPost_BlogCategory.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:63:[dbo].[Cart] in "c:\SQLAzureMW\BCPData\dbo.Cart.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:151:[dbo].[CartItem] in "c:\SQLAzureMW\BCPData\dbo.CartItem.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:4:[dbo].[Comments] in "c:\SQLAzureMW\BCPData\dbo.Comments.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[ContactInquiries] in "c:\SQLAzureMW\BCPData\dbo.ContactInquiries.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:5:[dbo].[ContactTypes] in "c:\SQLAzureMW\BCPData\dbo.ContactTypes.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:1:[dbo].[ContentNesting] in "c:\SQLAzureMW\BCPData\dbo.ContentNesting.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:12:[dbo].[ContentPage] in "c:\SQLAzureMW\BCPData\dbo.ContentPage.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:19:[dbo].[Customer] in "c:\SQLAzureMW\BCPData\dbo.Customer.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:10:[dbo].[DistributionCenters] in "c:\SQLAzureMW\BCPData\dbo.DistributionCenters.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:10:[dbo].[FAQ] in "c:\SQLAzureMW\BCPData\dbo.FAQ.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[FaqTopic] in "c:\SQLAzureMW\BCPData\dbo.FaqTopic.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:7:[dbo].[Invoice] in "c:\SQLAzureMW\BCPData\dbo.Invoice.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:8:[dbo].[InvoiceCode] in "c:\SQLAzureMW\BCPData\dbo.InvoiceCode.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:12:[dbo].[InvoiceItem] in "c:\SQLAzureMW\BCPData\dbo.InvoiceItem.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:8:[dbo].[Locations] in "c:\SQLAzureMW\BCPData\dbo.Locations.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:59:[dbo].[Modules] in "c:\SQLAzureMW\BCPData\dbo.Modules.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:2:[dbo].[Newsletter] in "c:\SQLAzureMW\BCPData\dbo.Newsletter.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:36:[dbo].[Payment] in "c:\SQLAzureMW\BCPData\dbo.Payment.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[PaymentType] in "c:\SQLAzureMW\BCPData\dbo.PaymentType.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:8:[dbo].[Profile] in "c:\SQLAzureMW\BCPData\dbo.Profile.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:417:[dbo].[ProfileModules] in "c:\SQLAzureMW\BCPData\dbo.ProfileModules.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:3:[dbo].[ScheduledTask] in "c:\SQLAzureMW\BCPData\dbo.ScheduledTask.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:1:[dbo].[Services] in "c:\SQLAzureMW\BCPData\dbo.Services.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:8:[dbo].[SettingGroup] in "c:\SQLAzureMW\BCPData\dbo.SettingGroup.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:63:[dbo].[Setting] in "c:\SQLAzureMW\BCPData\dbo.Setting.dat" -E -n -b 10000 -a 16384
GO
-- BCPArgs:35:[dbo].[Testimonial] in "c:\SQLAzureMW\BCPData\dbo.Testimonial.dat" -E -n -b 10000 -a 16384
GO

CREATE TABLE Shipment (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	order_id int NOT NULL,
	shipment_number varchar(255) NULL,
	tracking_number varchar(255) NULL,
	dateShipped datetime NULL,
	weight varchar(25) NULL
)
GO

CREATE TABLE FTPFirewall (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ipaddress varchar(255) NOT NULL
)
GO

CREATE TABLE InvoiceAddress (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	first varchar(255) NULL,
	last varchar(255) NULL,
	street1 varchar(500) NOT NULL,
	street2 varchar(400) NULL,
	city varchar(500) NOT NULL,
	state varchar(25) NOT NULL,
	postal_code varchar(100) NOT NULL,
	country varchar(100) NULL
)
GO