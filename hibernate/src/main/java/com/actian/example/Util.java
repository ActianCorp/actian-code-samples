package com.actian.example;

import org.hibernate.SessionFactory;
import org.hibernate.boot.Metadata;
import org.hibernate.boot.MetadataSources;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;
import org.hibernate.cfg.Configuration;

public class Util {
	private static StandardServiceRegistry registry;
	private static SessionFactory sessionFactory;

	public static SessionFactory getSessionFactory(boolean initData) {
		if (sessionFactory == null) {
			try {
				// Create registry
				if (initData) {
					registry = new StandardServiceRegistryBuilder().configure().build();
				}
				else {
					Configuration config = new Configuration();
					config.configure("hibernate.cfg.xml");
					config.setProperty("hibernate.hbm2ddl.auto", "update");
					registry = new StandardServiceRegistryBuilder().configure().applySettings(config.getProperties()).build();
				}

				// Create MetadataSources
				MetadataSources sources = new MetadataSources(registry);

				// Create Metadata
				Metadata metadata = sources.getMetadataBuilder().build();

				// Create SessionFactory
				sessionFactory = metadata.getSessionFactoryBuilder().build();

			} catch (Exception e) {
				e.printStackTrace();
				if (registry != null) {
					StandardServiceRegistryBuilder.destroy(registry);
				}
			}
		}
		return sessionFactory;
	}

	public static void shutdown() {
		if (registry != null) {
			StandardServiceRegistryBuilder.destroy(registry);
		}
	}
}
