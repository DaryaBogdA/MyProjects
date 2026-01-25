package com.example.Bank_REST;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

/**
 * Главный класс Spring Boot приложения для управления банковскими картами.
 * 
 * <p>Приложение предоставляет REST API для управления пользователями и банковскими картами
 * с использованием JWT аутентификации и Spring Security.</p>
 * 
 * @author darya
 * @version 1.0
 */
@SpringBootApplication
public class BankRestApplication {

	/**
	 * Точка входа в приложение.
	 * 
	 * @param args аргументы командной строки
	 */
	public static void main(String[] args) {
		SpringApplication.run(BankRestApplication.class, args);
	}

}
